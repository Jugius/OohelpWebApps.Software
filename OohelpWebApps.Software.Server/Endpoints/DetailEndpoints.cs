using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Server.Mapping;
using OohelpWebApps.Software.Server.Services;

namespace OohelpWebApps.Software.Server.Endpoints;

public static class DetailEndpoints
{
    public static void MapDetailEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/detail/{releaseId:guid}", CreateDetail).RequireAuthorization("InAdminRole");
        app.MapPut("/api/detail/{id:guid}", UpdateDetail).RequireAuthorization("InAdminRole");
        app.MapDelete("/api/detail/{id:guid}", DeleteDetail).RequireAuthorization("InAdminRole");
    }
    private static async Task<IResult> CreateDetail(Guid releaseId, ReleaseDetailRequest request, ApplicationsService appService)
    {
        var result = await appService.CreateDetail(releaseId, request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> UpdateDetail(Guid id, ReleaseDetailRequest request, ApplicationsService appService)
    {
        var result = await appService.UpdateDetail(id, request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> DeleteDetail(Guid id, ApplicationsService appService)
    {
        var result = await appService.DeleteDetail(id);

        return result.Match(
            s => Results.Ok(),
            f => result.Error.ToApiErrorResult());
    }
}
