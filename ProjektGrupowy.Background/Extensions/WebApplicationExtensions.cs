using Hangfire;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ProjektGrupowy.Application.Exceptions;
using ProjektGrupowy.Application.Filters;
using ProjektGrupowy.Application.Services;
using ProjektGrupowy.Application.SignalR;
using ProjektGrupowy.Infrastructure.Data;
using Serilog;

namespace ProjektGrupowy.Background.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication RegisterHangfireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = [new HangfireDashboardFilter()] });
        return app;
    }

    public static WebApplication RegisterHealth(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        return app;
    }
    
    public static WebApplication ConfigureSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) 
            return app;
        
        app.UseSwagger();
        app.UseSwaggerUI();
        
        return app;
    }

    public static WebApplication ConfigureBasicSettings(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseCors("FrontendPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        return app;
    }

    public static WebApplication ConfigureExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
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
                            StatusCode = context.Response.StatusCode,
                            Message = exception.Message
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        await messageService.SendErrorAsync(currentUserService.UserId, "An internal server error occurred.");

                        await context.Response.WriteAsJsonAsync(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "An internal server error occurred."
                        });
                    }
                }
            });
        });
        
        return app;
    }
    
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}