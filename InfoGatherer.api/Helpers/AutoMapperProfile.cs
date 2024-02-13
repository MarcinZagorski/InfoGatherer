using AutoMapper;
using InfoGatherer.api.Data.Entities.Scrapper;
using InfoGatherer.api.DTOs.Scrappers.Wibor;

namespace InfoGatherer.api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Wibor, WiborDto>().ReverseMap();
        }
    }
}
