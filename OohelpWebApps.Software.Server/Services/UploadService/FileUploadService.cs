using OohelpSoft.Utiles.Result;
using OohelpWebApps.Software.Server.Common.Interfaces;
using OohelpWebApps.Software.Server.Exceptions;

namespace OohelpWebApps.Software.Server.Services.UploadService;

public class FileUploadService : IUploadService
{
    private const string FilesFolder = "UploadedFiles";
    private readonly string _uploadDirectory;
    public FileUploadService(IWebHostEnvironment env)
    {
        _uploadDirectory = Path.Combine(env.ContentRootPath, FilesFolder);
    }
    
    public async Task<Result> SaveAsync(byte[] fileBytes, Guid fileId)
    {
        var fileName = Guider.ToStringFromGuid(fileId);
        var filePath = Path.Combine(_uploadDirectory, fileName);
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(fileBytes);
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }
    }

    public async Task<Result<byte[]>> GetFileAsync(Guid fileId)
    {
        var fileName = Guider.ToStringFromGuid(fileId);
        var filePath = Path.Combine(_uploadDirectory, fileName);

        if (!File.Exists(filePath))
            return ApiException.FileSystemError("File not exist");

        try
        {
            var bytes = await File.ReadAllBytesAsync(filePath);
            return bytes;
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError("File read error: " + ex.Message);
        }
    }

    public Task<Result<string>> GetDownloadUrlAsync(Guid fileId, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteFileAsync(Guid fileId)
    {
        var fileName = Guider.ToStringFromGuid(fileId);
        var filePath = Path.Combine(_uploadDirectory, fileName);

        if (!File.Exists(filePath)) return Result.SuccessAsync();

        try
        {
            File.Delete(filePath);
            return Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            return Result.FailureAsync(ApiException.FileSystemError(ex.GetBaseException().Message));
        }
    }
}
