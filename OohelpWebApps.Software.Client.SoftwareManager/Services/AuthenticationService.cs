using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SoftwareManager.Mapping;

namespace SoftwareManager.Services;
internal static class AuthenticationService
{
    public static async Task<ValueResult<string>> Login()
    {
        if (string.IsNullOrEmpty(AppSettings.Instance.Username) ||
           string.IsNullOrEmpty(AppSettings.Instance.Password) ||
           string.IsNullOrEmpty(AppSettings.Instance.AuthenticationApiServer))
            return new Exception("Ошибка аутентификации. В настройках приложения не хватает данных.");

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

        return await response.Content.ReadAsStringAsync();
    }
}
