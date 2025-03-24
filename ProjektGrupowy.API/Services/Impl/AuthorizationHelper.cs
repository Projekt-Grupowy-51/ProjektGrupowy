using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Models;
using ProjektGrupowy.API.Utils;
using ProjektGrupowy.API.Utils.Constants;
using System.Security.Claims;

namespace ProjektGrupowy.API.Services;

public class AuthorizationHelper : IAuthorizationHelper
{
    private readonly IScientistService _scientistService;
    private readonly IProjectService _projectService;
    private readonly ISubjectService _subjectService;
    private readonly IVideoGroupService _videoGroupService;
    private readonly IVideoService _videoService;
    private readonly ILabelService _labelService;
    private readonly ILabelerService _labelerService;
    private readonly IAssignedLabelService _assignedLabelService;
    private readonly ISubjectVideoGroupAssignmentService _subjectVideoGroupAssignmentService;

    public AuthorizationHelper(
        IScientistService scientistService, 
        IProjectService projectService, 
        ISubjectService subjectService,
        IVideoGroupService videoGroupService,
        IVideoService videoService,
        ILabelService labelService,
        ILabelerService labelerService,
        IAssignedLabelService assignedLabelService,
        ISubjectVideoGroupAssignmentService subjectVideoGroupAssignmentService)
    {
        _scientistService = scientistService;
        _projectService = projectService;
        _subjectService = subjectService;
        _videoGroupService = videoGroupService;
        _videoService = videoService;
        _labelService = labelService;
        _labelerService = labelerService;
        _assignedLabelService = assignedLabelService;
        _subjectVideoGroupAssignmentService = subjectVideoGroupAssignmentService;
    }

    private ActionResult CreateUnauthorizedIdentityResult() =>
        new UnauthorizedObjectResult("User identity not found.");
        
    private ActionResult? CheckResultSuccess<T>(Optional<T> result) => 
        result.IsSuccess ? null : new NotFoundObjectResult(result.GetErrorOrThrow());
        
    private async Task<ActionResult?> CheckScientistOwnership<T>(
        ClaimsPrincipal user, 
        int entityId,
        Func<int, Task<Optional<T>>> getEntityFunc,
        Func<T, int> getProjectIdFromEntity)
    {
        var checkResult = await CheckGeneralAccessAsync(user);
        
        if (checkResult.Error != null) return checkResult.Error;
        if (checkResult.IsAdmin) return null;

        if (!checkResult.IsScientist) return new ForbidResult();

        var entityResult = await getEntityFunc(entityId);
        if (!entityResult.IsSuccess)
            return new NotFoundObjectResult(entityResult.GetErrorOrThrow());

        var projectId = getProjectIdFromEntity(entityResult.GetValueOrThrow());
        
        var projectResult = await _projectService.GetProjectAsync(projectId);
        if (!projectResult.IsSuccess)
            return new NotFoundObjectResult(projectResult.GetErrorOrThrow());
        
        if (projectResult.GetValueOrThrow().Scientist.Id != checkResult.Scientist!.Id)
            return new ForbidResult();
        
        return null;
    }
    
    public string? GetUserId(ClaimsPrincipal user) => 
        user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public Task<bool> IsUserInRoleAsync(ClaimsPrincipal user, string role) => 
        Task.FromResult(user.IsInRole(role));
    
    public async Task<(ActionResult? Error, bool IsScientist, bool IsAdmin, bool IsLabeler, Scientist? Scientist, Labeler? Labeler)> CheckGeneralAccessAsync(ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        if (userId == null)
        {
            return (CreateUnauthorizedIdentityResult(), false, false, false, null, null);
        }

        bool isScientist = user.IsInRole(RoleConstants.Scientist);
        bool isAdmin = user.IsInRole(RoleConstants.Admin);
        bool isLabeler = user.IsInRole(RoleConstants.Labeler);

        Scientist? scientist = null;
        Labeler? labeler = null;

        if (isScientist)
        {
            var scientistResult = await _scientistService.GetScientistByUserIdAsync(userId);
            if (!scientistResult.IsSuccess)
            {
                return (new NotFoundObjectResult(scientistResult.GetErrorOrThrow()), false, false, false, null, null);
            }
            scientist = scientistResult.GetValueOrThrow();
        }

        if (isLabeler)
        {
            var labelerResult = await _labelerService.GetLabelerByUserIdAsync(userId);
            if (!labelerResult.IsSuccess)
            {
                return (new NotFoundObjectResult(labelerResult.GetErrorOrThrow()), isScientist, isAdmin, false, scientist, null);
            }
            labeler = labelerResult.GetValueOrThrow();
        }

        if (!isScientist && !isAdmin && !isLabeler)
        {
            return (new ForbidResult(), false, false, false, null, null);
        }

        return (null, isScientist, isAdmin, isLabeler, scientist, labeler);
    }
    
    public async Task<ActionResult?> EnsureScientistOwnsProjectAsync(ClaimsPrincipal user, int projectId)
    {
        return await CheckScientistOwnership(
            user,
            projectId,
            _projectService.GetProjectAsync,
            project => project.Id);
    }

    public async Task<ActionResult?> EnsureScientistOwnsSubjectAsync(ClaimsPrincipal user, int subjectId)
    {
        return await CheckScientistOwnership(
            user,
            subjectId,
            _subjectService.GetSubjectAsync,
            subject => subject.Project.Id);
    }

    public async Task<ActionResult?> EnsureScientistOwnsVideoGroupAsync(ClaimsPrincipal user, int videoGroupId)
    {
        return await CheckScientistOwnership(
            user,
            videoGroupId,
            _videoGroupService.GetVideoGroupAsync,
            videoGroup => videoGroup.Project.Id);
    }

    public async Task<ActionResult?> EnsureScientistOwnsVideoAsync(ClaimsPrincipal user, int videoId)
    {
        var videoResult = await _videoService.GetVideoAsync(videoId);
        if (!videoResult.IsSuccess)
            return new NotFoundObjectResult(videoResult.GetErrorOrThrow());
            
        return await EnsureScientistOwnsVideoGroupAsync(user, videoResult.GetValueOrThrow().VideoGroup.Id);
    }

    public async Task<ActionResult?> EnsureScientistOwnsLabelAsync(ClaimsPrincipal user, int labelId)
    {
        var labelResult = await _labelService.GetLabelAsync(labelId);
        if (!labelResult.IsSuccess)
            return new NotFoundObjectResult(labelResult.GetErrorOrThrow());
            
        return await EnsureScientistOwnsProjectAsync(user, labelResult.GetValueOrThrow().Subject.Project.Id);
    }

    public async Task<ActionResult?> EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(ClaimsPrincipal user, int assignmentId)
    {
        return await CheckScientistOwnership(
            user,
            assignmentId,
            _subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync,
            assignment => assignment.Subject.Project.Id);
    }
    
    public async Task<(ActionResult? Error, Labeler? Labeler)> GetLabelerFromUserAsync(ClaimsPrincipal user)
    {
        var userId = GetUserId(user);
        if (userId == null)
            return (CreateUnauthorizedIdentityResult(), null);

        if (!user.IsInRole(RoleConstants.Labeler))
            return (new ForbidResult(), null);

        var labelerResult = await _labelerService.GetLabelerByUserIdAsync(userId);
        if (!labelerResult.IsSuccess)
            return (new NotFoundObjectResult(labelerResult.GetErrorOrThrow()), null);

        return (null, labelerResult.GetValueOrThrow());
    }

    public async Task<bool> CanLabelerAccessAssignmentAsync(ClaimsPrincipal user, int assignmentId)
    {
        var labelerResult = await GetLabelerFromUserAsync(user);
        if (labelerResult.Error != null)
            return false;

        var assignmentResult = await _subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentAsync(assignmentId);
        if (!assignmentResult.IsSuccess)
            return false;

        var assignment = assignmentResult.GetValueOrThrow();
        
        return assignment.Labelers?.Any(x => x.Id == labelerResult.Labeler!.Id) ?? false;
    }

    public async Task<ActionResult?> EnsureLabelerCanAccessAssignmentAsync(ClaimsPrincipal user, int assignmentId)
    {
        var canAccess = await CanLabelerAccessAssignmentAsync(user, assignmentId);
        return canAccess ? null : new ForbidResult();
    }

    public async Task<ActionResult?> EnsureLabelerOwnsAssignedLabelAsync(ClaimsPrincipal user, int assignedLabelId)
    {
        var labelerResult = await GetLabelerFromUserAsync(user);
        if (labelerResult.Error != null)
            return labelerResult.Error;

        var assignedLabelResult = await _assignedLabelService.GetAssignedLabelAsync(assignedLabelId);
        if (!assignedLabelResult.IsSuccess)
            return new NotFoundObjectResult(assignedLabelResult.GetErrorOrThrow());

        var assignedLabel = assignedLabelResult.GetValueOrThrow();
        
        return assignedLabel.Labeler.Id == labelerResult.Labeler!.Id 
            ? null
            : new ForbidResult();
    }

    public async Task<bool> CanLabelerAccessVideoGroupAsync(ClaimsPrincipal user, int videoGroupId)
    {
        var labelerResult = await GetLabelerFromUserAsync(user);
        if (labelerResult.Error != null)
            return false;

        var assignments = await _subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(videoGroupId);
        if (!assignments.IsSuccess)
            return false;

        foreach (var assignment in assignments.GetValueOrThrow())
        {
            if (await CanLabelerAccessAssignmentAsync(user, assignment.Id))
                return true;
        }

        return false;
    }

    public async Task<ActionResult?> EnsureLabelerCanAccessVideoAsync(ClaimsPrincipal user, int videoId)
    {
        var labelerResult = await GetLabelerFromUserAsync(user);
        if (labelerResult.Error != null)
            return labelerResult.Error;

        var videoResult = await _videoService.GetVideoAsync(videoId);
        if (!videoResult.IsSuccess)
            return new NotFoundObjectResult(videoResult.GetErrorOrThrow());

        var videoGroupId = videoResult.GetValueOrThrow().VideoGroup.Id;
        
        var assignments = await _subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsByVideoGroupIdAsync(videoGroupId);
        if (!assignments.IsSuccess)
            return new NotFoundObjectResult(assignments.GetErrorOrThrow());
        
        foreach (var assignment in assignments.GetValueOrThrow())
        {
            if (await CanLabelerAccessAssignmentAsync(user, assignment.Id))
                return null;
        }
        
        return new ForbidResult();
    }

    public async Task<ActionResult?> EnsureLabelerCanAccessSubjectAsync(ClaimsPrincipal user, int subjectId)
    {
        var labelerResult = await GetLabelerFromUserAsync(user);
        if (labelerResult.Error != null)
            return labelerResult.Error;
            
        var subjectResult = await _subjectService.GetSubjectAsync(subjectId);
        if (!subjectResult.IsSuccess)
            return new NotFoundObjectResult(subjectResult.GetErrorOrThrow());
            
        var assignments = await _subjectVideoGroupAssignmentService.GetSubjectVideoGroupAssignmentsBySubjectIdAsync(subjectId);
        if (!assignments.IsSuccess)
            return new NotFoundObjectResult(assignments.GetErrorOrThrow());
            
        foreach (var assignment in assignments.GetValueOrThrow())
        {
            if (await CanLabelerAccessAssignmentAsync(user, assignment.Id))
                return null; 
        }
        
        return new ForbidResult();
    }
}
