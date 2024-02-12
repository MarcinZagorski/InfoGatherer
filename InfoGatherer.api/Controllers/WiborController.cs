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
    public class WiborController : ControllerBase
    {
        private const string APIKEYNAME = "ApiKey";

        // TODO: get_last_available && Post_list(pagination) && get_date / add cache
        [HttpGet("last_available")]
        public async Task<IActionResult> GetLastAvailableWibor()
        {
            await Task.Delay(1000);
            return Ok();
        }
        [HttpGet("{date}")]
        public async Task<IActionResult> GetWiborByDate(DateTime date)
        {
            await Task.Delay(1000);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> WiborList()
        {
            await Task.Delay(1000);
            return Ok();
        }
    }
}
