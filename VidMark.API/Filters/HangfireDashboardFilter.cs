using Hangfire.Dashboard;

namespace VidMark.API.Filters;

public sealed class HangfireDashboardFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext ctx) => true;
}