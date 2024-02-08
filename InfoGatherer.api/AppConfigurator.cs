using FluentValidation;
using InfoGatherer.api.Configuration;
using InfoGatherer.api.Data;
using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.Data.Repositories;
using InfoGatherer.api.DTOs.Users;
using InfoGatherer.api.Validators.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using NLog;
using ILogger = NLog.ILogger;
using NLog.Extensions.Logging;
using InfoGatherer.api.Services.Interfaces;
using InfoGatherer.api.Services;
using Hangfire;
using InfoGatherer.api.Filters;

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
            var logger = NLog.LogManager.GetCurrentClassLogger();
            services.AddSingleton<ILogger>(logger);

            // config's
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
            services.Configure<GeneralConfig>(configuration.GetSection("GeneralConfig"));
            services.Configure<HangfireConfig>(configuration.GetSection("HangfireConfig"));

            // repos
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // validators
            services.AddTransient<IValidator<UserRegisterDto>, UserRegisterDtoValidator>();
            services.AddTransient<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();


            // services
            services.AddScoped<ITokenService, TokenService>();





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
                c.SwaggerEndpoint("/api/swagger/public/swagger.json", "InfoGatherer API Public");
                if (app.Environment.IsDevelopment())
                {
                    c.SwaggerEndpoint("/api/swagger/internal/swagger.json", "InfoGatherer API Internal");
                }
            });
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            if (!string.IsNullOrWhiteSpace(app.Configuration["HangfireConfig:Enabled"]) && bool.Parse(app.Configuration["HangfireConfig:Enabled"]))
            {
                app.MapHangfireDashboard("/DashboardHf", new DashboardOptions { AppPath = null, Authorization = new[] { new HangfireAuthFilter() }, StatsPollingInterval = 30000, DashboardTitle = "InfoGatherer Tasks" });
            }

            // app.UseWebSockets(); // Later add sockets
            //app.MapHub<NotificationHub>("/NotificationSocket");
            app.UseHttpsRedirection();

            app.MapControllers();
        }
    }

}
