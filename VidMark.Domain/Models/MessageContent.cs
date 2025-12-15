namespace VidMark.Domain.Models;

public enum MessageContent
{
    AssignedLabelAdded,
    ReportGenerated,
    LabelAdded,
    LabelUpdated,
    ProjectAdded,
    ProjectUpdated,
    LabelerJoinedProject,
    AccessCodeRetired,
    AccessCodeCreated,
    SubjectAdded,
    SubjectUpdated,
    LabelersRemovedFromAssignment,
    LabelersAddedToAssignment,
    AssignmentCreated,
    AssignmentUpdated,
    LabelerAddedToAssignment,
    LabelerRemovedFromAssignment,
    AssignmentMarkedComplete,
    AssignmentMarkedIncomplete,
    VideoAdded,
    VideoUpdated,
    VideoProcessed,
    VideoGroupCreated,
    VideoGroupUpdated,
    EntityDeleted,
    GenericError,
    ValidationError
}