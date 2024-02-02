using NLog.Config;
using NLog;

public static class LoggerConfig
{
    public static void ConfigureLogger(WebApplicationBuilder builder)
    {
        var loggerConfig = new XmlLoggingConfiguration("nlog.config");
        LogManager.Configuration = loggerConfig;
        Logger logger = LogManager.GetCurrentClassLogger();
        logger.Trace($"ENVIRONMENT: {builder.Environment.EnvironmentName}");
    }
}
