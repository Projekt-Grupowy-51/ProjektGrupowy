namespace ProjektGrupowy.API.Services
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        bool IsAdmin { get; }
    }
}
