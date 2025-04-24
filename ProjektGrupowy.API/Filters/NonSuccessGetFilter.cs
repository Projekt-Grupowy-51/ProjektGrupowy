using Microsoft.AspNetCore.Mvc.Filters;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils.Extensions;

namespace ProjektGrupowy.API.Filters;

public class NonSuccessGetFilter(IMessageService messageService) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    { 
        var executedContext = await next();

        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        if (request.Method == HttpMethods.Get && (response.StatusCode < 200 || (response.StatusCode >= 300 && response.StatusCode != 302)))
        {
            try 
            {
                var identity = context.HttpContext.User.GetUserId();
                await messageService.SendErrorAsync(identity, "Something went wrong");
            } 
            catch (Exception ex)
            {
            }
        }
    }
}