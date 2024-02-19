using InfoGatherer.api.Services.Interfaces;

namespace InfoGatherer.api.BackgroundTasks.Jobs.Scrappers
{
    public interface IWiborJob : IBackgroundTask
    {
    }
    public class WiborJob([FromKeyedServices("Bankier")] IWiborScrapperService bankierScrapperService) : IWiborJob
    {
        private readonly IWiborScrapperService _bankierScrapperService = bankierScrapperService;

        public async Task<string> ExecuteAsync()
        {
            string res = await _bankierScrapperService.ScrapeAndSaveDataAsync();
            return String.Concat(DateTime.Now.ToString("yyyy-MM-dd"), " - ", res);
        }
    }
}
