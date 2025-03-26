namespace ProjektGrupowy.API.Utils.Constants;

public static class RoleConstants
{
    public const string Admin = "Admin";
    public const string Scientist = "Scientist";
    public const string Labeler = "Labeler";
    
    public static readonly IReadOnlyList<string> AllRoles = new List<string> 
    { 
        Admin, 
        Scientist, 
        Labeler 
    }.AsReadOnly();
}
