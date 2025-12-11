using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektGrupowy.Domain.Models;

// [Table("Videos")]
public class Video : BaseEntity, IOwnedEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Path { get; set; } = string.Empty;

    [Required]
    public virtual VideoGroup VideoGroup { get; set; } = default!;

    public int PositionInQueue { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public int VideoGroupId { get; set; }
    public virtual ICollection<AssignedLabel> AssignedLabels { get; set; } = new List<AssignedLabel>();
    public string CreatedById { get; set; } = string.Empty;

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = default!;
    public DateTime? DelDate { get; set; } = null;
    
    public string OriginalQuality { get; set; } = string.Empty;

    public string[] AvailableQualities { get; set; } = [];

    public Stream ToStream(string? quality = null)
    {
        try
        {
            var filePath = quality != null && AvailableQualities.Contains(quality)
                ? System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path) ?? string.Empty, $"{System.IO.Path.GetFileNameWithoutExtension(Path)}_{quality}{System.IO.Path.GetExtension(Path)}")
                : Path;

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception)
        {
            return new MemoryStream();
        }
    }

    public static Video Create(string title, string path, VideoGroup videoGroup, string contentType, int positionInQueue, string createdById)
    {
        var video = new Video
        {
            Title = title,
            Path = path,
            VideoGroup = videoGroup,
            ContentType = contentType,
            PositionInQueue = positionInQueue,
            CreatedById = createdById
        };
        video.AddDomainEvent("Wideo zostało dodane!", createdById);
        return video;
    }

    public void Update(string title, VideoGroup videoGroup, string? path, string? contentType, int positionInQueue, string userId)
    {
        Title = title;
        VideoGroup = videoGroup;
        PositionInQueue = positionInQueue;

        if (path != null)
        {
            Path = path;
        }

        if (contentType != null)
        {
            ContentType = contentType;
        }

        AddDomainEvent("Wideo zostało zaktualizowane!", userId);
    }
}