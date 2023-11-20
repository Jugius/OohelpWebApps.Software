﻿using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Server.Mapping;
using OohelpWebApps.Software.Server.Services;

namespace OohelpWebApps.Software.Server.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/file/{releaseId:guid}", CreateFile).RequireAuthorization("InAdminRole");
        app.MapDelete("/api/file/{id:guid}", DeleteFile).RequireAuthorization("InAdminRole");
        app.MapGet("/api/file/{id:guid}", DownloadFile);
    }
    private static async Task<IResult> CreateFile(Guid releaseId, ReleaseFileRequest request, ApplicationsService appService)
    {
        var result = await appService.CreateFile(releaseId, request);

        return result.Match(
            s => Results.Json(result.Value),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> DeleteFile(Guid id, ApplicationsService appService)
    {
        var result = await appService.DeleteFile(id);

        return result.Match(
            s => Results.Ok(),
            f => result.Error.ToApiErrorResult());
    }
    private static async Task<IResult> DownloadFile(Guid id, ApplicationsService appService)
    {
        var result = await appService.GetFileById(id);

        return result.Match(
            s => Results.File(result.Value.Bytes, "application/octet-stream", result.Value.FileName),
            f => result.Error.ToApiErrorResult());
    }
}
