using ProjektGrupowy.Application.DTOs.Messages;
using ProjektGrupowy.Application.Http;
using ProjektGrupowy.Domain.Models;
using ProjektGrupowy.Infrastructure.Repositories;

namespace ProjektGrupowy.Application.SignalR;

public class HttpMessageService(
    IHttpMessageClient messageClient,
    INotificationRepository notificationRepository) : IMessageService
{
    public async Task SendErrorAsync(string userId, string message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Error, message, cancellationToken);

    public async Task SendInfoAsync(string userId, string message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Info, message, cancellationToken);

    public async Task SendSuccessAsync(string userId, string message, CancellationToken cancellationToken = default) => 
        await SendMessageAsync(userId, MessageTypes.Success, message, cancellationToken);
        
    public async Task SendWarningAsync(string userId, string message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.Warning, message, cancellationToken);

    public async Task SendRegularMessageAsync(string userId, string message, CancellationToken cancellationToken = default) =>
        await SendMessageAsync(userId, MessageTypes.Message, message, cancellationToken);
    
    public async Task SendMessageAsync(string userId, string method, string message,
        CancellationToken cancellationToken = default)
    {
        var messageObject = new HttpNotification(userId, method, message);
        var messageEntity = new Notification
        {
            RecipientId = userId,
            Type = method,
            Message = message
        };
        
        await Task.WhenAll(
            messageClient.SendMessageAsync(messageObject, cancellationToken), 
            notificationRepository.AddNotificationAsync(messageEntity));
    }
}