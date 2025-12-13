namespace VidMark.Application.Features.SubjectVideoGroupAssignments.Queries.GetAssignmentStatistics;

public class AssignmentStatistics
{
    public int TotalVideos { get; set; }
    public int TotalLabels { get; set; }
    public int AssignedLabelersCount { get; set; }
    public double CompletionPercentage { get; set; }
    public List<VideoProgress> Videos { get; set; } = new();
    public Dictionary<string, int> TopLabelers { get; set; } = new();
    public List<LabelerStat> AllLabelers { get; set; } = new();
    public VideoStatusCounts VideoStatus { get; set; } = new();
}

public class VideoProgress
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int LabelsReceived { get; set; }
    public int ExpectedLabels { get; set; }
    public double CompletionPercentage { get; set; }
}

public class LabelerStat
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int LabelCount { get; set; }
    public double CompletionPercentage { get; set; }
}

public class VideoStatusCounts
{
    public int Completed { get; set; }
    public int InProgress { get; set; }
    public int NotStarted { get; set; }
}
