using InfoGatherer.api.Data.Entities.Scrapper;
using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Scrappers.Wibor;
using InfoGatherer.api.Filters;
using InfoGatherer.api.Helpers;
using InfoGatherer.api.Models;
using InfoGatherer.api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoGatherer.api.Controllers
{
    [ApiExplorerSettings(GroupName = "public")]
    [Authorize(AuthenticationSchemes = "BearerLocal")]
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    [ApiKeyAuth]
    public class WiborController(IWiborRepository repo, IMemoryCacheService crepo) : ControllerBase
    {
        private const string APIKEYNAME = "ApiKey";
        private readonly IWiborRepository _repo = repo;
        private readonly IMemoryCacheService _crepo = crepo;

        // TODO: Post_list(pagination) / add cache
        [HttpGet("last_available")]
        public async Task<IActionResult> GetLastAvailableWibor()
        {
            WiborDto x = await _repo.GetLastWibor();
            if (x == null) { return NotFound("Did not found specifed record"); }
            return Ok(x);
        }
        [HttpGet("{date}")]
        public async Task<IActionResult> GetWiborByDate(DateTime date)
        {
            var cacheKey = $"wibor{date:yyyyMMdd}";
            var wiborData = _crepo.Get<WiborDto>(cacheKey);
            if (wiborData == null)
            {    
                wiborData = await _repo.GetWiborByDate(date);

                if (wiborData == null)
                {
                    return NotFound($"Did not found any data for {date:yyyy-MM-dd}");
                }
                else
                {
                    
                    _crepo.Set(cacheKey, wiborData, TimeSpan.FromHours(4));
                }
            }

            return Ok(wiborData);
        }
        [HttpPost]
        public async Task<IActionResult> WiborList(PaginationListModel<DefaultFilter> opt)
        {
            IQueryable<Wibor> query = _repo.GetQuerableList();
            //if (!string.IsNullOrWhiteSpace(opt.Filter.Phrase)) 
            //{
            //    query = query.Where(x=> x.Date.ToString() == opt.Filter.Phrase);
            //}
            var list = await PaginationExtensions.GetPage(query, opt);
            return Ok(list);
        }
    }
}
