using Minio;
using Minio.DataModel.Args;
using OohelpSoft.Utiles.Result;
using OohelpWebApps.Software.Server.Common.Interfaces;
using OohelpWebApps.Software.Server.Configurations;
using OohelpWebApps.Software.Server.Exceptions;

namespace OohelpWebApps.Software.Server.Services.UploadService;

public class MinioUploadService : IUploadService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _projectName;
    public MinioUploadService(MinioOptions options)
    {
        _bucketName = options.BucketName;
        _projectName = options.ProjectName;
        _minioClient = new MinioClient()
            .WithEndpoint(options.Endpoint)
            .WithCredentials(options.AccessKey, options.SecretKey)
            .WithSSL()
            .Build();
    }
    public async Task<Result> SaveAsync(byte[] fileBytes, Guid fileId)
    {
        var objectName = $"{_projectName}/{fileId}";
        using var stream = new MemoryStream(fileBytes);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType("application/octet-stream");

        try
        {
            await _minioClient.PutObjectAsync(putObjectArgs);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }
    }
    public async Task<Result<byte[]>> GetFileAsync(Guid fileId)
    {
        var objectName = $"{_projectName}/{fileId}";
        using var ms = new MemoryStream();
        var getArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(async (stream, cancellationToken) => await stream.CopyToAsync(ms, cancellationToken));

        try
        {
            await _minioClient.GetObjectAsync(getArgs);            
            return ms.ToArray();
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }        
    }
    public async Task<Result<string>> GetDownloadUrlAsync(Guid fileId, string fileName)
    {
        var objectName = $"{_projectName}/{fileId}";
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithExpiry(60 * 60) // 1 hour
            .WithHeaders (new Dictionary<string, string>
            {
                ["response-content-disposition"] = $"attachment; filename=\"{fileName}\""
            });

        try
        {
            return await _minioClient.PresignedGetObjectAsync(args);
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }
    }
    public async Task<Result> DeleteFileAsync(Guid fileId)
    {
        var objectName = $"{_projectName}/{fileId}";
        
        try
        {
            // Проверяем, существует ли файл
            await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName));

            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName));

            return Result.Success();
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            // Файл не найден — ничего не делаем, просто возвращаемся
            return Result.Success();
        }
        catch (Exception ex)
        {
            return ApiException.FileSystemError(ex.GetBaseException().Message);
        }
        
    }



    


}
