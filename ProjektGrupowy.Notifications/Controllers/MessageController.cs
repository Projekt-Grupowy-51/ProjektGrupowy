using Microsoft.AspNetCore.Mvc;
using ProjektGrupowy.Application.DTOs.Messages;
using ProjektGrupowy.Application.SignalR;

namespace ProjektGrupowy.Notifications.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(
    IMessageService messageService,
    ILogger<MessageController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SubmitAsync(HttpNotification notification)
    {
        try
        {
            await messageService.SendMessageAsync(notification.UserId, notification.Type, notification.Message);
            return Accepted();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message: {Message}", notification);
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occurred while processing your request: {ex.Message}");
        }
    }
}