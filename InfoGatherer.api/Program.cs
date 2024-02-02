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
using InfoGatherer.api.Configuration;
using InfoGatherer.api;

var builder = WebApplication.CreateBuilder(args);

LoggerConfig.ConfigureLogger(builder);
Logger logger = LogManager.GetCurrentClassLogger();

try
{
    var appConfigurator = new AppConfigurator(builder);
    appConfigurator.ConfigureServices();
    appConfigurator.Configure(builder.Environment);
    var app = builder.Build();
    appConfigurator.ConfigurePipeline(app);
    string adminEmail = appConfigurator.Configuration["GeneralConfig:AdminEmail"];
    string adminDefaultPassword = appConfigurator.Configuration["GeneralConfig:AdminDefaultPassword"];

    app.SetupIdentity(adminEmail, adminDefaultPassword);
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
