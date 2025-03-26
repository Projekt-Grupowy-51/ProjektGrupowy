using ProjektGrupowy.API.DTOs.Project;

namespace ProjektGrupowy.API.DTOs.Scientist;

public class ScientistResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
