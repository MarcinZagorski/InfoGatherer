using InfoGatherer.api.Configuration;
using InfoGatherer.api.Data;
using InfoGatherer.api.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace InfoGatherer.api
{
    public class AppConfigurator
    {
        private readonly WebApplicationBuilder _builder;
        public IConfiguration Configuration { get; }
        public AppConfigurator(WebApplicationBuilder builder)
        {
            _builder = builder;
            Configuration = builder.Configuration;
        }

        public void ConfigureServices()
        {
            var services = _builder.Services;
            var configuration = _builder.Configuration;

            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
            services.Configure<GeneralConfig>(configuration.GetSection("GeneralConfig"));
            services.Configure<HangfireConfig>(configuration.GetSection("HangfireConfig"));

            services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddScoped<DbContext>(provider => provider.GetService<AppDbContext>());

            JwtConfigurator.ConfigureJwt(services, configuration);
            HangfireConfigurator.ConfigureHangfire(services, configuration);
            DatabaseConfigurator.ConfigureDatabase(services, configuration);
            services.AddIdentity<AppUser, Role>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();
        }
        public void Configure(IWebHostEnvironment env)
        {
            var services = _builder.Services;
            services.AddEndpointsApiExplorer();

            SwaggerConfigurator.ConfigureSwagger(services, env);
        }

        public void ConfigurePipeline(WebApplication app)
        {
            app.UsePathBase("/api");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/public/swagger.json", "InfoGatherer API Public");
                if (app.Environment.IsDevelopment())
                {
                    c.SwaggerEndpoint("/swagger/internal/swagger.json", "InfoGatherer API Internal");
                }
            });
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            // app.UseWebSockets(); // Later add sockets
            //app.MapHub<NotificationHub>("/NotificationSocket");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
    }

}
