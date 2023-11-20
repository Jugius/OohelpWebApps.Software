using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Server.Mapping;
using OohelpWebApps.Software.Server.Services;

namespace OohelpWebApps.Software.Server.Endpoints;

public static class ReleaseEndpoints
{
    public static void MapReleaseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/release/{applicationId:guid}", CreateRelease).RequireAuthorization("InAdminRole");
        app.MapPut("/api/release/{id:guid}", UpdateRelease).RequireAuthorization("InAdminRole");
        app.MapDelete("/api/release/{id:guid}", DeleteRelease).RequireAuthorization("InAdminRole");
    }
    private static async Task<IResult> CreateRelease(Guid applicationId, ReleaseRequest request, ApplicationsService appService)
    {
        var result = await appService.CreateRelease(applicationId, request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> UpdateRelease(Guid id, ReleaseRequest request, ApplicationsService appService)
    {
        var result = await appService.UpdateRelease(id, request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> DeleteRelease(Guid id, ApplicationsService appService)
    {
        var result = await appService.DeleteRelease(id);

        return result.Match(
            s => Results.Ok(),
            f => result.Error.ToApiErrorResult());
    }
}
