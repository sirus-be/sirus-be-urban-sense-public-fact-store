using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace FactStore.Jobs
{
    public class DashboardAuthorizationFilter: IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return context.GetHttpContext().User.Identity.IsAuthenticated;
        }
    }
}
