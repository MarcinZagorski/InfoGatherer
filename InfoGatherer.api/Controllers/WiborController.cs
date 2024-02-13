using InfoGatherer.api.Data.Repositories.Interfaces;
using InfoGatherer.api.DTOs.Scrappers.Wibor;
using InfoGatherer.api.Filters;
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
    public class WiborController(IWiborRepository repo) : ControllerBase
    {
        private const string APIKEYNAME = "ApiKey";
        private readonly IWiborRepository _repo = repo;

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
            WiborDto x = await _repo.GetWiborByDate(date);
            if (x == null) { return NotFound($"Did not found any data for {date}"); }
            return Ok(x);
        }
        [HttpPost]
        public async Task<IActionResult> WiborList()
        {
            await Task.Delay(1000);
            return Ok();
        }
    }
}
