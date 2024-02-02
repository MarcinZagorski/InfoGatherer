using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ILogger = NLog.ILogger;

namespace InfoGatherer.api.Controllers
{
    [ApiExplorerSettings(GroupName = "internal")]
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "BearerLocal")]
    public class BaseController : ControllerBase
    {
        protected ILogger _logger = LogManager.GetCurrentClassLogger();
    }
}
