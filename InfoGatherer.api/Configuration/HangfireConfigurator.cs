using Hangfire;
using InfoGatherer.api.BackgroundTasks;
using InfoGatherer.api.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;

public static class HangfireConfigurator
{
    public static void ConfigureHangfire(IServiceCollection services, IConfiguration configuration)
    {
        var hangfireConfig = configuration.GetSection("HangfireConfig").Get<HangfireConfig>();
        if (hangfireConfig == null || !hangfireConfig.Enabled) return;

        services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("dbConnection")));
        services.AddHangfireServer();
        //services.AddHangfireServer(options => options.WorkerCount = 1); // for async jobs

        var backgroundTaskTypes = Assembly.GetExecutingAssembly().GetTypes()
           .Where(t => typeof(IBackgroundTask).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).ToList();

        foreach (var taskType in backgroundTaskTypes)
        {
            services.AddScoped(taskType);
        }
    }
    public static void ScheduleHangfireJobs(IServiceProvider serviceProvider, HangfireConfig hangfireConfig)
    {
        var taskTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IBackgroundTask).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        foreach (var taskType in taskTypes)
        {
            var taskStatus = hangfireConfig.Tasks.FirstOrDefault(x => x.Name == taskType.Name);
            if (taskStatus == null || !taskStatus.Enabled) continue;

            var cronExpression = GenerateCronExpression(taskStatus);
            if (string.IsNullOrEmpty(cronExpression)) continue;

            using (var scope = scopeFactory.CreateScope())
            {
                var taskInstance = scope.ServiceProvider.GetService(taskType) as IBackgroundTask;
                if (taskInstance == null) continue;

                RecurringJob.AddOrUpdate(taskStatus.Name, () => taskInstance.ExecuteAsync(), cronExpression);
            }
        }
    }

    private static string GenerateCronExpression(InfoGatherer.api.Configuration.TaskStatus taskStatus)
    {
        if (taskStatus.IntervalExecution)
        {
            return $"*/{taskStatus.Minute} * * * *";
        }
        else
        {
            return $"{taskStatus.Minute} {taskStatus.Hour} * * *";
        }
    }
}
