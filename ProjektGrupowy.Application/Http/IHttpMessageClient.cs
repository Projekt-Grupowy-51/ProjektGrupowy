using ProjektGrupowy.Application.DTOs.Messages;

namespace ProjektGrupowy.Application.Http;

public interface IHttpMessageClient
{
    Task SendMessageAsync(HttpNotification notification, CancellationToken cancellationToken = default);
}