using System.Text.Json.Serialization;
using OohelpWebApps.Software.Domain;
using OohelpWebApps.Software.Server.Exceptions;
using OohelpWebApps.Software.Server.Mapping;
using OohelpWebApps.Software.Server.ObsoleteRequests;
using OohelpWebApps.Software.Server.Services;

namespace OohelpWebApps.Software.Server.Endpoints;

public static class UpdaterEndpoints
{
    public static void MapUpdaterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/software/api/updates/NewReleases", GetNewerAppReleases);
        app.MapPost("/software/api/updates/LastRelease", GetLastRelease);
        app.MapGet("/software/api/files/download/{id:guid}", DownloadFile);
    }
    private static async Task<IResult> GetNewerAppReleases(GetNewerReleasesRequest request, ApplicationsService applicationsService)
    {
        var applicationResult = await applicationsService.GetApplicationByName(request.Name);
        if (!applicationResult.IsSuccess) return ExceptionToResult(applicationResult.Error);

        var releases = applicationResult.Value.Releases.Where(a => a.Version > request.CurrentVersion);
        return ReleasesToResult(releases);
    }
    private static async Task<IResult> GetLastRelease(GetLastReleaseRequest request, ApplicationsService applicationsService)
    {
        var applicationResult = await applicationsService.GetApplicationByName(request.Name);
        if (!applicationResult.IsSuccess) return ExceptionToResult(applicationResult.Error);

        var releases = applicationResult.Value.Releases;

        var release = request.RuntimeVersion.HasValue
            ? releases.Where(a => a.Files.Any(b => b.RuntimeVersion == request.RuntimeVersion.Value)).MaxBy(a => a.Version)
            : releases.MaxBy(a => a.Version);

        return ReleaseToResult(release);
    }
    private static async Task<IResult> DownloadFile(Guid id, ApplicationsService appService)
    {
        var result = await appService.GetFileById(id);

        return result.Match(
            s => Results.File(result.Value.Bytes, "application/octet-stream", result.Value.FileName),
            f => result.Error.ToApiErrorResult());
    }



    private static IResult ExceptionToResult(Exception exception)
    {
        var reason = exception is ApiException apiException ? apiException.Reason : ExceptionReason.UnknownError;
        var result = new
        {
            Status = reason.ToString(),
            ErrorMessage = exception.Message
        };
        return Results.Json(result);
    }
    private static IResult ReleasesToResult(IEnumerable<ApplicationRelease> releases)
    {
        var result = new
        {
            Status = "Ok",
            Releases = releases.ToArray()
        };
        return Results.Json(result, new System.Text.Json.JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
    }
    private static IResult ReleaseToResult(ApplicationRelease release)
    {
        var result = new
        {
            Status = "Ok",
            Release = release
        };
        return Results.Json(result, new System.Text.Json.JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        });
    }
}
