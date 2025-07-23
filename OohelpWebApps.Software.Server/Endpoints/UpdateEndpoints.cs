using OohelpWebApps.Software.Server.Mapping;
using OohelpWebApps.Software.Server.Services;
using Serilog;

namespace OohelpWebApps.Software.Server.Endpoints;

public static class UpdateEndpoints
{
    public static void MapUpdateEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/update/{id:guid}", DownloadUpdate);
    }
    private static async Task<IResult> DownloadUpdate(Guid id, ApplicationsService appService)
    {
        var result = await appService.BuildUpdatePackage(id);

        if (result.IsFailure) return result.Error.ToApiErrorResult();

        var package = result.Value;

        Log.Information("Updated application {AppName}, release {Version} file {FileName}", package.Application.ApplicationName, package.Application.ReleaseVersion, package.Application.FileName);

        return Results.Json(package);
    }
}
