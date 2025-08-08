using OohelpSoft.Utiles.Result;

namespace OohelpWebApps.Software.Server.Common.Interfaces;

public interface IUploadService
{
    Task<Result> SaveAsync(byte[] fileBytes, Guid fileId);
    Task<Result<byte[]>> GetFileAsync(Guid fileId);
    Task<Result<string>> GetDownloadUrlAsync(Guid fileId, string fileName);
    Task<Result> DeleteFileAsync(Guid fileId);
}
