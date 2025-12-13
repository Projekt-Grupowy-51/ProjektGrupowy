using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace VidMark.Infrastructure.SignalR;

[Authorize]
public class AppHub : Hub;
