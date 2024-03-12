using OohelpSoft.Helpers.Result;
using OohelpWebApps.Software.Domain;

namespace OohelpWebApps.Software.Client.SoftwareManagerWeb.Services;

public class SoftwareApiService
{
    private readonly HttpClient _httpClient;
    private readonly string BaseUri;

    public SoftwareApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        BaseUri = configuration["Servers:ApiSoftware"];
    }
    public async Task<OperationResult<List<ApplicationInfo>>> GetAllApplications()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ApplicationInfo>>($"{BaseUri}/api/application");
        }
        catch (Exception ex)
        {
            return ex;
        }
         
    }
}
