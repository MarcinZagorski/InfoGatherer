using Hangfire;
using InfoGatherer.api.Data;
using InfoGatherer.api.Data.Entities;
using InfoGatherer.api.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Config;
using NLog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var loggerConfig = new XmlLoggingConfiguration("nlog.config");
LogManager.Configuration = loggerConfig;
Logger logger = LogManager.GetCurrentClassLogger();
try
{
    logger.Trace($"ENVIRONMENT: {builder.Environment.EnvironmentName}");
    ConfigurationManager Configuration = builder.Configuration;
    IWebHostEnvironment Env = builder.Environment;
    // Add services to the container.
    //builder.Services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
    //builder.Services.Configure<GeneralConfig>(Configuration.GetSection("GeneralConfig"));
    //builder.Services.Configure<HangfireConfig>(Configuration.GetSection("HangfireConfig"));
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
    {
        o.DocumentFilter<UrlRenameDocumentFilter>();
    });
    builder.Services.AddHttpContextAccessor();


    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(Configuration.GetConnectionString("dbConnection") ?? throw new InvalidOperationException("Connection String error"));
    });
    logger.Trace("AppDbContext service registered");

    builder.Services.AddScoped<DbContext>(provider => provider.GetService<AppDbContext>());
    var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
    var tokenValidationParams = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = Configuration["JwtConfig:TokenIssuer"],
        ValidateAudience = false,
        ValidateLifetime = true,
        RequireExpirationTime = false,
        ClockSkew = TimeSpan.Zero
    };
    builder.Services.AddSingleton(tokenValidationParams);
    builder.Services.AddAuthentication()
    .AddJwtBearer("BearerLocal", jwt =>
    {
        var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParams;
    });
    builder.Services.AddIdentity<AppUser, Role>(options =>
    {
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
    })
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<AppDbContext>();

    builder.Services.AddControllers();
    builder.Services.AddControllers().AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(swagger =>
    {
        swagger.SwaggerDoc("public", new OpenApiInfo { Title = "infoGatherer.api", Version = "public" });

        if (Env.IsDevelopment())
        {
            swagger.SwaggerDoc("internal", new OpenApiInfo { Title = "infoGatherer.api", Version = "internal" });
        }
        var bearerScheme = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
        swagger.AddSecurityDefinition("Bearer", bearerScheme);
        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement { { bearerScheme, new string[] { } } });

        var apiKeyScheme = new OpenApiSecurityScheme
        {
            Name = "ApiKey",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKey",
            In = ParameterLocation.Header,
            Description = "Enter your valid apiKey in the text input below.\r\n\r\nExample: \"5D91FDE66DB94B2584779BDB8652D6E9\"",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            }
        };
        swagger.AddSecurityDefinition("ApiKey", apiKeyScheme);
        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement { { apiKeyScheme, new string[] { } } });
    });

    // Uproszczenie logiki w³¹czania Hangfire
    bool.TryParse(Configuration["GeneralConfig:EnableBackgroundTasks"], out var hangfireEnabled);

    if (hangfireEnabled)
    {
        // Konfiguracja Hangfire
        builder.Services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(Configuration.GetConnectionString("dbConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
                  {
                      CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                      SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                      QueuePollInterval = TimeSpan.Zero,
                      UseRecommendedIsolationLevel = true,
                      DisableGlobalLocks = true
                  });
        });

        // Konfiguracja serwera Hangfire
        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
            options.ServerName = "InfoGatherer Server";
        });
    }


    var app = builder.Build();
    IServiceProvider services = app.Services.GetRequiredService<IServiceProvider>();
    // Configure the HTTP request pipeline.

    app.UsePathBase("/api");

    // Configure the HTTP request pipeline.
    if (Env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        if (Env.IsDevelopment())
            c.SwaggerEndpoint("internal/swagger.json", "infoGatherer.api (internal)");
        c.SwaggerEndpoint("public/swagger.json", "infoGatherer.api (public)");
    });
    app.UseRouting();
    // app.UseWebSockets(); // Later add sockets
    //app.MapHub<NotificationHub>("/NotificationSocket");
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    bool.TryParse(builder.Configuration["GeneralConfig:EnableBackgroundTasks"], out var hangfireDashboardEnabled);

    if (hangfireDashboardEnabled)
    {
        var hangfireDashboardOptions = new DashboardOptions
        {
            AppPath = null,
            Authorization = new[] { new HangfireAuthFilter() },
            StatsPollingInterval = 30000,
            DashboardTitle = "InfoGatherer Server"
        };
        app.MapHangfireDashboard("/hf", hangfireDashboardOptions);
    }



    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}