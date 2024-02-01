using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace InfoGatherer.api.Filters
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;//TODO: secure later
        }
    }
}