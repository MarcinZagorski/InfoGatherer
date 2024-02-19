namespace InfoGatherer.api.Services.Interfaces
{
    public interface IWiborScrapperService
    {
        Task<string> ScrapeAndSaveDataAsync();
    }
}
