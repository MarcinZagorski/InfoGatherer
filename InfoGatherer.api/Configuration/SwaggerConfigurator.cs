namespace InfoGatherer.api.Configuration
{
    using InfoGatherer.api.Filters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using System;

    public static class SwaggerConfigurator
    {
        public static void ConfigureSwagger(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("public", new OpenApiInfo { Title = "InfoGatherer.api", Version = "public" });

                if (env.IsDevelopment())
                {
                    swagger.SwaggerDoc("internal", new OpenApiInfo { Title = "InfoGatherer.api", Version = "internal" });
                }
                
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new string[] {}
                    }
                });

                swagger.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. ApiKey: Your_API_Key",
                    In = ParameterLocation.Header,
                    Name = "ApiKey",
                    Type = SecuritySchemeType.ApiKey
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            In = ParameterLocation.Header,
                            Name = "ApiKey",
                            Type = SecuritySchemeType.ApiKey
                        },
                        new string[] {}
                    }
                });
                swagger.DocumentFilter<UrlRenameDocumentFilter>();
            });
        }
    }
}
