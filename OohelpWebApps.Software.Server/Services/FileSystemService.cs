using OohelpSoft.Helpers.Result;
using OohelpWebApps.Software.Server.Exceptions;
using System.Text;

namespace OohelpWebApps.Software.Server.Services;

public class FileSystemService
{
    private const string FilesFolder = "UploadedFiles";
    private readonly string _uploadDirectory;
    public FileSystemService(IWebHostEnvironment env)
    {
        this._uploadDirectory = Path.Combine(env.ContentRootPath, FilesFolder);
    }

    public Task<Result<bool>> SaveFile(byte[] fileBytes, Guid fileId)
    {
        string fileName = Guider.ToStringFromGuid(fileId);
        return SaveFile(fileBytes, fileName);
    }
    public async Task<Result<byte[]>> GetFileBytes(Guid fileId)
    {
        string fileName = Guider.ToStringFromGuid(fileId);
        string filePath = Path.Combine(_uploadDirectory, fileName);

        if (!System.IO.File.Exists(filePath))
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
    public async Task<Result<bool>> SaveFile(byte[] fileBytes, string fileName)
    {
        string filePath = Path.Combine(_uploadDirectory, fileName);
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await stream.WriteAsync(fileBytes);
            }
            return true;
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }
    }

    public Result<bool> DeleteFile(Guid fileId)
    {
        string fileName = Guider.ToStringFromGuid(fileId);
        return DeleteFile(fileName);
    }
    public Result<bool> DeleteFile(string fileName)
    {
        string filePath = Path.Combine(_uploadDirectory, fileName);

        if (!System.IO.File.Exists(filePath))
            return ApiException.FileSystemError("File not exist");

        try
        {
            System.IO.File.Delete(filePath);
            return true;
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }
    }
    public string GetCheckSum(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(stream);
        return MakeHashString(hash);
    }
    private static string MakeHashString(byte[] hash)
    {
        StringBuilder s = new StringBuilder(hash.Length * 2);

        foreach (byte b in hash)
            s.Append(b.ToString("X2").ToLower());

        return s.ToString();
    }
}
