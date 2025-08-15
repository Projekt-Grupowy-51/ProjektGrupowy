using ProjektGrupowy.Application.DTOs.Messages;

namespace ProjektGrupowy.Application.Http;

public interface IHttpMessageClient
{
    Task SendMessageAsync(HttpMessage message, CancellationToken cancellationToken = default);
}