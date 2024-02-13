using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using InfoGatherer.api.Data.Repositories.Interfaces;

namespace InfoGatherer.api.Filters
{
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey);
            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();

            var response = await userRepository.CheckApiKey(extractedApiKey);

            if (response == null)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Unathorized"
                };
                return;
            }

            await next();
        }
    }
}