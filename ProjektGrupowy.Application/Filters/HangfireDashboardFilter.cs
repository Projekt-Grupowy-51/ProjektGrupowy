using Hangfire.Dashboard;

namespace ProjektGrupowy.Application.Filters;

public sealed class HangfireDashboardFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext ctx) => true;
}