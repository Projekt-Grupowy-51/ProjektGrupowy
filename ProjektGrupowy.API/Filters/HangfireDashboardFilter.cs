using Hangfire.Dashboard;

namespace ProjektGrupowy.API.Filters;

public sealed class HangfireDashboardFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext ctx) => true;
}