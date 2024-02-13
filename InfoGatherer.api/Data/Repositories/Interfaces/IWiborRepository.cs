using InfoGatherer.api.DTOs.Scrappers.Wibor;

namespace InfoGatherer.api.Data.Repositories.Interfaces
{
    public interface IWiborRepository
    {
        Task<WiborDto> GetLastWibor();
        Task<WiborDto> GetWiborByDate(DateTime date);
    }
}
