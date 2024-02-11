namespace InfoGatherer.api.BackgroundTasks
{
    public interface IBackgroundTask
    {
        Task<string> ExecuteAsync();
    }
}
