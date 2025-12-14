using Microsoft.AspNetCore.Mvc.Filters;
using VidMark.Application.Interfaces.SignalR;
using VidMark.Application.Services;
using VidMark.Domain.Models;
using Serilog;

namespace VidMark.API.Filters;

public class NonSuccessGetFilter(IMessageService messageService, ICurrentUserService currentUserService) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var executedContext = await next();

        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        if (response.StatusCode >= 400)
        {
            try
            {
                await messageService.SendErrorAsync(currentUserService.UserId, MessageContent.GenericError);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred: {Error}", ex.Message);
            }
        }
    }
}