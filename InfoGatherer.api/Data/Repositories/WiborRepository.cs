using AutoMapper;
using InfoGatherer.api.Data.Entities.Scrapper;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Scrappers.Wibor;
using Microsoft.EntityFrameworkCore;

namespace InfoGatherer.api.Data.Repositories
{
    public class WiborRepository(AppDbContext ctx, IMapper mapper) : IWiborRepository
    {
        private readonly AppDbContext _ctx = ctx;
        private readonly IMapper _mapper = mapper;
        public async Task<WiborDto> GetLastWibor()
        {
            Wibor w = await _ctx.Wibors.AsQueryable().OrderByDescending(x => x.Date).FirstOrDefaultAsync();
            return _mapper.Map<WiborDto>(w);
        }
        public async Task<WiborDto> GetWiborByDate(DateTime date)
        {
            Wibor w = await _ctx.Wibors.FirstOrDefaultAsync(x => x.Date == date);
            return _mapper.Map<WiborDto>(w);
        }
        public IQueryable<Wibor> GetQuerableList()
        {
            return _ctx.Wibors.AsQueryable();
        }
        public async Task<string> AddOrUpdate(Wibor inputWibor)
        {
            var existingWibor = await _ctx.Wibors.FirstOrDefaultAsync(w => w.Date.Date == inputWibor.Date.Date);

            if (existingWibor == null)
            {
                _ctx.Wibors.Add(inputWibor);
                await _ctx.SaveChangesAsync();
                return "Added";
            }
            else
            {

                existingWibor.Overnight = inputWibor.Overnight;
                existingWibor.TomorrowNext = inputWibor.TomorrowNext;
                existingWibor.SpotWeek = inputWibor.SpotWeek;
                existingWibor.OneMonth = inputWibor.OneMonth;
                existingWibor.ThreeMonths = inputWibor.ThreeMonths;
                existingWibor.SixMonths = inputWibor.SixMonths;
                existingWibor.OneYear = inputWibor.OneYear;
                existingWibor.CreatedAt = DateTime.Now;

                await _ctx.SaveChangesAsync();
                return "Updated";
            }
        }
    }
}
