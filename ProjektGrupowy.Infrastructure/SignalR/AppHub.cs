using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ProjektGrupowy.Infrastructure.SignalR;

[Authorize]
public class AppHub : Hub
{
}
