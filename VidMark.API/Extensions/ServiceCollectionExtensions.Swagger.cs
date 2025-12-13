using System.Reflection;
using Microsoft.OpenApi.Models;
using VidMark.API.Filters;

namespace VidMark.API.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

            foreach (var xml in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
                c.IncludeXmlComments(xml, includeControllerXmlComments: true);

            c.SchemaFilter<EnumDescriptionsSchemaFilter>();

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VidMark API",
                Version = "v1",
                Description = "API for VidMark application",
                Contact = new OpenApiContact
                {
                    Name = "Support",
                    Email = "support@VidMark.com"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
