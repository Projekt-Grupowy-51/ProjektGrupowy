using System.Security.Cryptography;
using System.Text;

namespace VidMark.Application.Helpers;

public static class NginxSignedUrl
{
    public static string Sign(string publicBaseUrl, string uriPath, DateTimeOffset expiresUtc, string secret)
    {
        var expires = expiresUtc.ToUnixTimeSeconds().ToString();
        var toSign = $"{expires}{uriPath} {secret}";
        var hash   = MD5.HashData(Encoding.ASCII.GetBytes(toSign));
        var sig = Convert.ToBase64String(hash).TrimEnd('=').Replace('+','-').Replace('/','_');

        return $"{publicBaseUrl}{uriPath}?md5={sig}&expires={expires}";
    }
}