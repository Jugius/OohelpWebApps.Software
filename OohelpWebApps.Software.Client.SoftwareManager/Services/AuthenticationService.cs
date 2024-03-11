using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using OohelpSoft.Helpers.Result;
using SoftwareManager.Mapping;

namespace SoftwareManager.Services;
internal static class AuthenticationService
{
    public static async Task<OperationResult<string>> Login()
    {
        if (string.IsNullOrEmpty(AppSettings.Instance.Username) ||
           string.IsNullOrEmpty(AppSettings.Instance.Password) ||
           string.IsNullOrEmpty(AppSettings.Instance.AuthenticationApiServer))
            return new Exception("Ошибка аутентификации. В настройках приложения не хватает данных.");

        if (IsValidToken(AppSettings.Instance.Token))
            return AppSettings.Instance.Token;

        using HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri($"https://{AppSettings.Instance.AuthenticationApiServer}")
        };

        var json = new
        {
            Email = AppSettings.Instance.Username,
            Password = AppSettings.Instance.Password,
        };

        var response = await httpClient.PostAsJsonAsync("/api/user/login", json);

        if (!response.IsSuccessStatusCode) return await response.ToHttpException("Ошибка аутентификации");

        AppSettings.Instance.Token = await response.Content.ReadAsStringAsync();
        AppSettings.Save(AppSettings.Instance);
        return AppSettings.Instance.Token;
    }
    private static bool IsValidToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        var tokenDate = GetTokenExpirationTime(token);
        var now = DateTime.Now.ToUniversalTime();

        return tokenDate >= now;
    }
    private static DateTime GetTokenExpirationTime(string token)
    {
        var payload = token.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var tokenExp = keyValuePairs["exp"];
        var ticks = long.Parse(tokenExp.ToString());

        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;

        return tokenDate;
    }
    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
