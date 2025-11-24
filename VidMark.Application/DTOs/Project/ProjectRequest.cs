using System.ComponentModel.DataAnnotations;

namespace VidMark.Application.DTOs.Project;

/// <summary>
/// DTO for project request - update or create project
/// </summary>
public class ProjectRequest
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    [Required(ErrorMessage = "Podanie nazwy projektu jest wymagane.")]
    [StringLength(255, ErrorMessage = "Maksymalna długość nazwy projektu wynosi 255 znaków.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A brief description of the project.
    /// </summary>
    [Required(ErrorMessage = "Podanie opisu projektu jest wymagane.")]
    [StringLength(1000, ErrorMessage = "Maksymalna długość opisu projektu wynosi 1000 znaków.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the project is finished.
    /// </summary>
    public bool Finished { get; set; }
}