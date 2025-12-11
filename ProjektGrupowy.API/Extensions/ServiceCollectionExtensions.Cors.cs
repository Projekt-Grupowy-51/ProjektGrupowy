namespace ProjektGrupowy.API.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                var allowedDevOrigin = configuration["Cors:AllowedDevOrigin"];
                var allowedProdOrigin = configuration["Cors:AllowedProductionOrigin"];

                policy.WithOrigins(allowedDevOrigin!, allowedProdOrigin!)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
