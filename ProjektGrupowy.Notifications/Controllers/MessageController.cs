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
    public async Task<IActionResult> SubmitAsync(HttpMessage message)
    {
        try
        {
            await messageService.SendMessageAsync(message.UserId, message.Type, message.Message);
            return Accepted();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message: {Message}", message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occurred while processing your request: {ex.Message}");
        }
    }
}