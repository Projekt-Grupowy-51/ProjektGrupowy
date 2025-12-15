using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VidMark.Application.Interfaces.SignalR;
using VidMark.Application.Services;
using VidMark.Domain.Models;
using VidMark.API.Utils;
using VidMark.API.Extensions;

namespace VidMark.API.Filters;

public class ValidateModelStateFilter(IMessageService messageService, ICurrentUserService currentUserService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState.GetErrorsAsResponse());
            await messageService.SendErrorAsync(currentUserService.UserId, MessageContent.ValidationError);
            return;
        }

        await next();
    }
}
