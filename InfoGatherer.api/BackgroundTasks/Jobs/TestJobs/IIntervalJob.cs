namespace InfoGatherer.api.BackgroundTasks.Jobs.TestJobs
{
    public interface IIntervalJob : IBackgroundTask
    {
    }
    public class IntervalJob : IIntervalJob
    {
        public async Task<string> ExecuteAsync()
        {
            await Task.Delay(1000);
            return String.Concat(DateTime.Now.ToString("yyyy-MM-dd"), " IntervalJob Oks");
        }
    }
}
