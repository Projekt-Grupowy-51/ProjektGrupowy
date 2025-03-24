using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.API.Models;
using System.Security.Claims;

namespace ProjektGrupowy.API.Services;

public interface IAuthorizationHelper
{
    // User identity and role methods
    string? GetUserId(ClaimsPrincipal user);
    Task<bool> IsUserInRoleAsync(ClaimsPrincipal user, string role);
    
    // Core authorization check methods
    Task<(ActionResult? Error, bool IsScientist, bool IsAdmin, bool IsLabeler, Scientist? Scientist, Labeler? Labeler)> CheckGeneralAccessAsync(ClaimsPrincipal user);
    Task<(ActionResult? Error, Labeler? Labeler)> GetLabelerFromUserAsync(ClaimsPrincipal user);
    // Scientist ownership verification methods
    Task<ActionResult?> EnsureScientistOwnsProjectAsync(ClaimsPrincipal user, int projectId);
    Task<ActionResult?> EnsureScientistOwnsSubjectAsync(ClaimsPrincipal user, int subjectId);
    Task<ActionResult?> EnsureScientistOwnsVideoGroupAsync(ClaimsPrincipal user, int videoGroupId);
    Task<ActionResult?> EnsureScientistOwnsVideoAsync(ClaimsPrincipal user, int videoId);
    Task<ActionResult?> EnsureScientistOwnsLabelAsync(ClaimsPrincipal user, int labelId);
    Task<ActionResult?> EnsureScientistOwnsSubjectVideoGroupAssignmentAsync(ClaimsPrincipal user, int assignmentId);
    
    // Labeler access verification methods
    Task<bool> CanLabelerAccessAssignmentAsync(ClaimsPrincipal user, int assignmentId);
    Task<ActionResult?> EnsureLabelerCanAccessAssignmentAsync(ClaimsPrincipal user, int assignmentId);
    Task<ActionResult?> EnsureLabelerOwnsAssignedLabelAsync(ClaimsPrincipal user, int assignedLabelId);
    Task<bool> CanLabelerAccessVideoGroupAsync(ClaimsPrincipal user, int videoGroupId);
    Task<ActionResult?> EnsureLabelerCanAccessVideoAsync(ClaimsPrincipal user, int videoId);
    Task<ActionResult?> EnsureLabelerCanAccessSubjectAsync(ClaimsPrincipal user, int subjectId);
}
