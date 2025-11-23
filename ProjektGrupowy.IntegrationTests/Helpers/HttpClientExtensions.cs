using ProjektGrupowy.IntegrationTests.Infrastructure;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ProjektGrupowy.IntegrationTests.Helpers;

public static class HttpClientExtensions
{
    /// <summary>
    /// Adds test authentication headers to the HTTP client
    /// </summary>
    public static HttpClient WithAuthenticatedUser(
        this HttpClient client,
        string? userId = null,
        string? userName = null,
        params string[] roles)
    {
        client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.UserIdHeaderName);
        client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.UserNameHeaderName);
        client.DefaultRequestHeaders.Remove(TestAuthenticationHandler.RolesHeaderName);

        var actualUserId = userId ?? Guid.NewGuid().ToString();
        var actualUserName = userName ?? "test-user";

        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserIdHeaderName, actualUserId);
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserNameHeaderName, actualUserName);

        if (roles.Length > 0)
        {
            client.DefaultRequestHeaders.Add(
                TestAuthenticationHandler.RolesHeaderName,
                string.Join(",", roles));
        }

        return client;
    }

    /// <summary>
    /// Adds Admin role authentication
    /// </summary>
    public static HttpClient WithAdminUser(this HttpClient client, string? userId = null)
    {
        return client.WithAuthenticatedUser(userId, "admin-user", "Admin");
    }

    /// <summary>
    /// Adds Scientist role authentication
    /// </summary>
    public static HttpClient WithScientistUser(this HttpClient client, string? userId = null)
    {
        return client.WithAuthenticatedUser(userId, "scientist-user", "Scientist");
    }

    /// <summary>
    /// Adds Labeler role authentication
    /// </summary>
    public static HttpClient WithLabelerUser(this HttpClient client, string? userId = null)
    {
        return client.WithAuthenticatedUser(userId, "labeler-user", "Labeler");
    }

    /// <summary>
    /// Posts JSON content to the specified URI
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T content)
    {
        var json = JsonSerializer.Serialize(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(requestUri, httpContent);
    }

    /// <summary>
    /// Puts JSON content to the specified URI
    /// </summary>
    public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T content)
    {
        var json = JsonSerializer.Serialize(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PutAsync(requestUri, httpContent);
    }

    /// <summary>
    /// Reads the response content as JSON and deserializes it
    /// </summary>
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
