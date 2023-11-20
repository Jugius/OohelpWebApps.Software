using Microsoft.EntityFrameworkCore;
using OohelpSoft.Helpers.Result;
using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Domain;
using OohelpWebApps.Software.Server.Database;
using OohelpWebApps.Software.Server.Exceptions;
using OohelpWebApps.Software.Server.Mapping;

namespace OohelpWebApps.Software.Server.Services;

public class ApplicationsService
{
    private readonly AppDbContext _dbContext;
    private readonly FileSystemService _fileSystemService;

    public ApplicationsService(AppDbContext context, FileSystemService fileSystemService)
    {
        _dbContext = context;
        _fileSystemService = fileSystemService;
    }
    public async Task<OperationResult<List<ApplicationInfo>>> GetAllApplications()
    {
        try
        {
            var apps = await _dbContext.Applications
            .AsNoTracking()
            .Include(a => a.Releases).ThenInclude(a => a.Details).AsSplitQuery()
            .Include(a => a.Releases).ThenInclude(a => a.Files).AsSplitQuery()
            .ToListAsync();

            return apps.Select(a => a.ToDomain()).ToList();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ApplicationInfo>> GetApplicationByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return ApiException.InvalidRequest("Name required");
        try
        {
            var app = await _dbContext.Applications
             .AsNoTracking()
             .Include(a => a.Releases).ThenInclude(a => a.Details).AsSplitQuery()
             .Include(a => a.Releases).ThenInclude(a => a.Files).AsSplitQuery()
             .FirstOrDefaultAsync(a => a.Name == name);

            if (app == null) return ApiException.NotFound();

            return app.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ApplicationInfo>> GetApplicationByName(string name, Version version)
    {
        if (string.IsNullOrEmpty(name)) return ApiException.InvalidRequest("Name required");

        var appResult = await GetApplicationByName(name);
        if (!appResult.IsSuccess) return appResult.Error;

        var application = appResult.Value;

        var releases = application.Releases.Where(a => a.Version >= version).ToList();
        application.Releases = releases;
        return application;
    }
    public async Task<OperationResult<ApplicationInfo>> CreateApplication(ApplicationRequest request)
    {
        var dto = request.ToDto();
        try
        {
            _dbContext.Applications.Add(dto);
            await _dbContext.SaveChangesAsync();
            return dto.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ApplicationInfo>> UpdateApplication(Guid applicationId, ApplicationRequest request)
    {
        try
        {
            var app = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app == null) return ApiException.NotFound();

            app.UpdateWith(request);

            await _dbContext.SaveChangesAsync();
            return app.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }
    public async Task<OperationResult<bool>> DeleteApplication(Guid applicationId)
    {
        try
        {
            var app = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app == null) return ApiException.NotFound();

            var filesIds = await _dbContext.Releases.AsNoTracking()
                .Where(a => a.ApplicationId == app.Id)
                .SelectMany(a => a.Files)
                .Select(a => a.Id)
                .ToListAsync();

            foreach (var id in filesIds)
            {
                _fileSystemService.DeleteFile(id);
            }

            _dbContext.Applications.Remove(app);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ApplicationRelease>> CreateRelease(Guid applicationId, ReleaseRequest request)
    {
        var release = request.ToDto();
        release.ApplicationId = applicationId;
        try
        {
            _dbContext.Releases.Add(release);
            await _dbContext.SaveChangesAsync();
            return release.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ApplicationRelease>> UpdateRelease(Guid releaseId, ReleaseRequest request)
    {
        try
        {
            var release = await _dbContext.Releases.FirstOrDefaultAsync(a => a.Id == releaseId);

            if (release == null) return ApiException.NotFound();

            release.UpdateWith(request);

            await _dbContext.SaveChangesAsync();
            return release.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<bool>> DeleteRelease(Guid releaseId)
    {
        try
        {
            var release = await _dbContext.Releases.FirstOrDefaultAsync(a => a.Id == releaseId);

            if (release == null) return ApiException.NotFound();

            var filesIds = await _dbContext.Files.AsNoTracking()
                .Where(a => a.ReleaseId == release.Id)
                .Select(a => a.Id)
                .ToListAsync();

            foreach (var id in filesIds)
            {
                _fileSystemService.DeleteFile(id);
            }

            _dbContext.Releases.Remove(release);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ReleaseDetail>> CreateDetail(Guid releaseId, ReleaseDetailRequest request)
    {
        var detail = request.ToDto();
        detail.ReleaseId = releaseId;
        try
        {
            _dbContext.Details.Add(detail);
            await _dbContext.SaveChangesAsync();
            return detail.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ReleaseDetail>> UpdateDetail(Guid detailId, ReleaseDetailRequest request)
    {
        try
        {
            var detail = await _dbContext.Details.FirstOrDefaultAsync(a => a.Id == detailId);
            if (detail == null) return ApiException.NotFound();

            detail.UpdateWith(request);

            await _dbContext.SaveChangesAsync();
            return detail.ToDomain();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<bool>> DeleteDetail(Guid detailId)
    {
        try
        {
            var detail = await _dbContext.Details.FirstOrDefaultAsync(a => a.Id == detailId);
            if (detail == null) return ApiException.NotFound();

            _dbContext.Details.Remove(detail);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }

    public async Task<OperationResult<ReleaseFile>> CreateFile(Guid releaseId, ReleaseFileRequest request)
    {
        var file = request.ToDto();

        file.ReleaseId = releaseId;
        file.CheckSum = _fileSystemService.GetCheckSum(request.FileBytes);
        file.Size = request.FileBytes.Length;

        try
        {
            _dbContext.Files.Add(file);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }

        var saveFileResult = await _fileSystemService.SaveFile(request.FileBytes, file.Id);
        if (saveFileResult.IsSuccess)
        {
            return file.ToDomain();
        }
        else
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
            return saveFileResult.Error;
        }
    }

    public async Task<OperationResult<bool>> DeleteFile(Guid fileId)
    {
        try
        {
            var file = await _dbContext.Files.FirstOrDefaultAsync(a => a.Id == fileId);
            if (file == null) return ApiException.NotFound();

            var delFileRes = _fileSystemService.DeleteFile(file.Id);

            if (delFileRes.IsSuccess)
            {
                _dbContext.Files.Remove(file);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return delFileRes;
            }
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }
    public async Task<OperationResult<FileBytes>> GetFileById(Guid fileId)
    {
        try
        {
            var file = await _dbContext.Files.FirstOrDefaultAsync(a => a.Id == fileId);
            if (file == null) return ApiException.NotFound();

            var bytesResult = await _fileSystemService.GetFileBytes(fileId);
            if (!bytesResult.IsSuccess) return bytesResult.Error;

            return new FileBytes(bytesResult.Value, file.Name);
        }
        catch (Exception ex)
        {
            return ApiException.DatabaseError(ex.GetBaseException().Message);
        }
    }
    internal async Task<OperationResult<ApplicationRelease>> FindLatestAppRelease(string name, FileRuntimeVersion? runtimeVersion)
    {
        var app = await GetApplicationByName(name);
        if (!app.IsSuccess) return app.Error;

        var release = runtimeVersion.HasValue
            ? app.Value.Releases.Where(a => a.Files.Any(b => b.RuntimeVersion == runtimeVersion.Value)).MaxBy(a => a.Version)
            : app.Value.Releases.MaxBy(a => a.Version);

        return release;
    }
}
