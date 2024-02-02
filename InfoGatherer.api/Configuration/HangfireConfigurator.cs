using Hangfire;
using Microsoft.Extensions.DependencyInjection;

public static class HangfireConfigurator
{
    public static void ConfigureHangfire(IServiceCollection services, IConfiguration configuration)
    {
        bool.TryParse(configuration["HangfireConfig:Enabled"], out var hangfireEnabled);

        if (hangfireEnabled)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UseSqlServerStorage(configuration.GetConnectionString("dbConnection"), new Hangfire.SqlServer.SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.Zero,
                          UseRecommendedIsolationLevel = true,
                          DisableGlobalLocks = true
                      });
            });

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 1;
                options.ServerName = "InfoGatherer Server";
            });
        }
    }
}
