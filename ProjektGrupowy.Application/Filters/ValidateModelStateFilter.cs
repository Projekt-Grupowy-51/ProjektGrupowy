using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.Utils;
using ProjektGrupowy.Infrastructure.Services;

namespace ProjektGrupowy.API.Filters;

public class ValidateModelStateFilter(IMessageService messageService, ICurrentUserService currentUserService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState.GetErrorsAsResponse());
            await messageService.SendErrorAsync(currentUserService.UserId, $"Error occured while validating Model. Errors: {context.ModelState.GetErrorsAsResponse()}");
            return;
        }

        await next();
    }
}
