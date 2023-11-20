using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Server.Mapping;
using OohelpWebApps.Software.Server.Services;

namespace OohelpWebApps.Software.Server.Endpoints;

public static class ApplicationEndpoints
{
    public static void MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/application", GetAllApplications).RequireAuthorization("InAdminRole");
        app.MapGet("/api/application/{name}", GetApplicationByName);
        app.MapPost("/api/application", CreateApplication).RequireAuthorization("InAdminRole");
        app.MapPut("/api/application/{id:guid}", UpdateApplication).RequireAuthorization("InAdminRole");
        app.MapDelete("/api/application/{id:guid}", DeleteApplication).RequireAuthorization("InAdminRole");
    }
    private static async Task<IResult> GetApplicationByName(string name, string version, ApplicationsService appService)
    {
        var result = !string.IsNullOrWhiteSpace(version) && Version.TryParse(version, out var resultVer)
                ? await appService.GetApplicationByName(name, resultVer)
                : await appService.GetApplicationByName(name);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> GetAllApplications(ApplicationsService appService)
    {
        var result = await appService.GetAllApplications();

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> CreateApplication(ApplicationRequest request, ApplicationsService appService)
    {
        var result = await appService.CreateApplication(request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> UpdateApplication(Guid id, ApplicationRequest request, ApplicationsService appService)
    {
        var result = await appService.UpdateApplication(id, request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> DeleteApplication(Guid id, ApplicationsService appService)
    {
        var result = await appService.DeleteApplication(id);

        return result.Match(
            s => Results.Ok(),
            f => result.Error.ToApiErrorResult());
    }
}
