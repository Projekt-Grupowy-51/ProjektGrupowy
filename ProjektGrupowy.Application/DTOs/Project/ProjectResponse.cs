namespace ProjektGrupowy.Application.DTOs.Project;

/// <summary>
/// DTO for project response - simple representation of a project
/// </summary>
public class ProjectResponse
{
    /// <summary>
    /// The unique identifier of the project.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the project.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The description of the project.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// The unique identifier of the scientist who owns the project. This is a GUID/UUID string.
    /// </summary>
    public string ScientistId { get; set; }
    
    /// <summary>
    /// The name of the scientist who owns the project.
    /// </summary>
    public DateOnly CreationDate { get; set; }
    
    /// <summary>
    /// The date when the project was last modified. Null if never modified.
    /// </summary>
    public DateOnly? ModificationDate { get; set; }
    
    /// <summary>
    /// The date when the project was finished. Null if not finished.
    /// </summary>
    public DateOnly? EndDate { get; set; }
    
    /// <summary>
    /// Indicates whether the project is finished.
    /// </summary>
    public bool Finished { get; set; }
}