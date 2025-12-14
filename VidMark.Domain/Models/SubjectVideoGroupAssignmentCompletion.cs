namespace VidMark.Domain.Models;

/// <summary>
/// Tracks completion status of SubjectVideoGroupAssignment by individual labelers.
/// Allows labelers to mark their work as complete independently.
/// </summary>
public class SubjectVideoGroupAssignmentCompletion : BaseEntity
{
    public int Id { get; set; }

    public int SubjectVideoGroupAssignmentId { get; set; }

    public virtual SubjectVideoGroupAssignment Assignment { get; set; } = default!;

    public string LabelerId { get; set; } = string.Empty;

    public virtual User Labeler { get; set; } = default!;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public static SubjectVideoGroupAssignmentCompletion Create(
        int assignmentId,
        string labelerId,
        string userId)
    {
        var completion = new SubjectVideoGroupAssignmentCompletion
        {
            SubjectVideoGroupAssignmentId = assignmentId,
            LabelerId = labelerId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        //completion.AddDomainEvent("Assignment completion record created", userId);
        return completion;
    }

    public void MarkAsCompleted(string userId)
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(MessageContent.AssignmentMarkedComplete, userId);
        }
    }

    public void MarkAsIncomplete(string userId)
    {
        if (IsCompleted)
        {
            IsCompleted = false;
            CompletedAt = null;
            ModifiedAt = DateTime.UtcNow;
            AddDomainEvent(MessageContent.AssignmentMarkedIncomplete, userId);
        }
    }
}
