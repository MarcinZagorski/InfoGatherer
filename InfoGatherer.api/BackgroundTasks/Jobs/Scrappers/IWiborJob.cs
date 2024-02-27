using InfoGatherer.api.Services.Interfaces;

namespace InfoGatherer.api.BackgroundTasks.Jobs.Scrappers
{
    public interface IWiborJob : IBackgroundTask
    {
    }
    public class WiborJob([FromKeyedServices("Bankier")] IWiborScrapperService bankierScrapperService, [FromKeyedServices("Money")] IWiborScrapperService moneyScrapperService) : IWiborJob
    {
        private readonly IWiborScrapperService _bankierScrapperService = bankierScrapperService;
        private readonly IWiborScrapperService _moneyScrapperService = moneyScrapperService;

        public async Task<string> ExecuteAsync()
        {
            string ban = await _bankierScrapperService.ScrapeAndSaveDataAsync();
            string mon = await _moneyScrapperService.ScrapeAndSaveDataAsync();
            return String.Concat(DateTime.Now.ToString("yyyy-MM-dd"), " - ", ban, " / ", mon);
        }
    }
}
