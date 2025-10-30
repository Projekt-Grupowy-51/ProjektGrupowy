using Hangfire;
using Microsoft.AspNetCore.Diagnostics;
using ProjektGrupowy.API.Filters;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Interfaces.SignalR;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Infrastructure.SignalR;
using Serilog;

namespace ProjektGrupowy.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseHangfireDashboardConfiguration(this WebApplication app)
    {
        _ = app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = [new HangfireDashboardFilter()] });

        // Register Hangfire jobs after the application has started and JobStorage is configured
        Application.IoC.ServiceCollectionExtensions.RegisterHangfireJobs();

        return app;
    }

    public static WebApplication UseHealthChecksConfiguration(this WebApplication app)
    {
        _ = app.MapHealthChecks("/health");
        return app;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        _ = app.UseSwagger();
        _ = app.UseSwaggerUI();

        return app;
    }

    public static WebApplication UseBasicConfiguration(this WebApplication app)
    {
        _ = app.UseHttpsRedirection();
        _ = app.UseCors("FrontendPolicy");
        _ = app.UseAuthentication();
        _ = app.UseAuthorization();
        _ = app.MapControllers();
        _ = app.MapHub<AppHub>("/signalr");

        return app;
    }

    public static WebApplication UseExceptionHandlingConfiguration(this WebApplication app)
    {
        _ = app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                using var scope = app.Services.CreateScope();
                var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
                var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    Log.Error("An error occurred: {Error}", exception);

                    if (exception is ForbiddenException)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;

                        await messageService.SendErrorAsync(currentUserService.UserId, "You do not have permission to perform this action.");

                        await context.Response.WriteAsJsonAsync(new
                        {
                            context.Response.StatusCode,
                            exception.Message
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        await messageService.SendErrorAsync(currentUserService.UserId, "An internal server error occurred.");

                        await context.Response.WriteAsJsonAsync(new
                        {
                            context.Response.StatusCode,
                            Message = "An internal server error occurred."
                        });
                    }
                }
            });
        });

        return app;
    }
}