using Microsoft.AspNetCore.Mvc.Filters;
using ProjektGrupowy.API.SignalR;
using ProjektGrupowy.API.Utils.Extensions;
using Serilog;

namespace ProjektGrupowy.API.Filters;

public class NonSuccessGetFilter(IMessageService messageService) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    { 
        var executedContext = await next();

        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        if (request.Method == HttpMethods.Get && response.StatusCode is >= 400 and < 500)
        {
            try 
            {
                var identity = context.HttpContext.User.GetUserId();
                await messageService.SendErrorAsync(identity, "Something went wrong");
            } 
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred: {Error}", ex.Message);
            }
        }
    }
}