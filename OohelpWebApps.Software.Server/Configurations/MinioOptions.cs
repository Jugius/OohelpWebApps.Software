namespace OohelpWebApps.Software.Server.Configurations;

public class MinioOptions
{
    public const string Key = "Minio";
    public string Endpoint { get; set; } = default!;
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public string BucketName { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
}
