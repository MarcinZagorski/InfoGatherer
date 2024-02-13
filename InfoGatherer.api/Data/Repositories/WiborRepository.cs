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
            Wibor w = await _ctx.Wibors.AsQueryable().OrderByDescending(x=>x.Date).FirstOrDefaultAsync();
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
    }
}
