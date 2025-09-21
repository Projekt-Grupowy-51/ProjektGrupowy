using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using ProjektGrupowy.Application.DTOs.Messages;

namespace ProjektGrupowy.Application.Http;

public class HttpMessageClient(HttpClient httpClient, IConfiguration configuration) : IHttpMessageClient
{
    public async Task SendMessageAsync(HttpNotification notification, CancellationToken cancellationToken = default)
    {
        var url = configuration["MessageService:Url"]!;
        var response = await httpClient.PostAsJsonAsync(url, notification, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}