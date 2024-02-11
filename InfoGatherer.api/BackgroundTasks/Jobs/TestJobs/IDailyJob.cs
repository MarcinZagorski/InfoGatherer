namespace InfoGatherer.api.BackgroundTasks.Jobs.TestJobs
{
    public interface IDailyJob : IBackgroundTask
    {
    }
    public class DailyJob : IDailyJob
    { 
    public async Task<string> ExecuteAsync()
        {
            await Task.Delay(1000);
            return String.Concat(DateTime.Now.ToString("yyyy-MM-dd"), " DailyJob Oks");
        }
    }
}
