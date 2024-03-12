using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OohelpSoft.Helpers.Result;
using OohelpWebApps.Software.Client.SoftwareManagerWeb.ViewModels;

namespace OohelpWebApps.Software.Client.SoftwareManagerWeb.Services;

public class AuthenticationProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorage;
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;

    public AuthenticationProvider(ILocalStorageService localStorage, IConfiguration configuration, HttpClient httpClient)
    {
        this.localStorage = localStorage;
        this.configuration = configuration;
        this.httpClient = httpClient;
    }

    private readonly ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string token = await localStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(anonymous);

            if (ValidateToken(token, out JwtSecurityToken securityToken))
            {
                var claimsPrincipal = SetClaimsPrincipal(securityToken.Claims);
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return new AuthenticationState(claimsPrincipal);
            }
            else
            {
                await localStorage.RemoveItemAsync("token");
                httpClient.DefaultRequestHeaders.Authorization = null;
                return new AuthenticationState(anonymous);
            }
        }
        catch
        {
            return new AuthenticationState(anonymous);
        }
    }
    public async Task UpdateAuthenticationStateAsync(string jwtToken)
    {
        var claimsPrincipal = new ClaimsPrincipal();
        if(!string.IsNullOrEmpty(jwtToken))
        {
            await localStorage.SetItemAsStringAsync("token", jwtToken);
            var claims = ParseClaimsFromJwtWithHandler(jwtToken);
            claimsPrincipal = SetClaimsPrincipal(claims);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        }
        else
        {
            if (await localStorage.ContainKeyAsync("token"))
                await localStorage.RemoveItemAsync("token");
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    private bool ValidateToken(string jwtToken, out JwtSecurityToken securityToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            }, out SecurityToken validatedToken);
            securityToken = (JwtSecurityToken)validatedToken;
            return securityToken != null;
        }
        catch
        {
            securityToken = null;
            return false;
        }
    }
    public async Task<OperationResult> Login(LoginViewModel request)
    {
        try
        {
            string loginUri = configuration["Servers:ApiUsers"] + "/api/user/login";
            using var resultMessage = await httpClient.PostAsJsonAsync(loginUri, request);

            if (resultMessage.IsSuccessStatusCode)
            {
                var token = await resultMessage.Content.ReadAsStringAsync();
                await UpdateAuthenticationStateAsync(token);
                return OperationResult.Success;
            }
            else
            {
                if (resultMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return new Exception("Invalid login or password");
                return new Exception($"{resultMessage.StatusCode}");
            }
        }
        catch (HttpRequestException httpUnavailable)
        {
            return new Exception($"Ошибка соединения с api: {httpUnavailable.HttpRequestError}");
        }
        catch (Exception ex)
        {
            return ex;
        }

    }
    public static ClaimsPrincipal SetClaimsPrincipal(IEnumerable<Claim> claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
    }

    private IEnumerable<Claim> ParseClaimsFromJwtWithHandler(string jwtToken)
    { 
        _ = ValidateToken(jwtToken, out JwtSecurityToken securityToken);
        return securityToken.Claims;
    }   
}
