namespace OohelpWebApps.Software.Server.Services;

public class FileBytes
{
    public string ApplicationName { get; set; }
    public string ReleaseVersion { get; set; }
    public byte[] Bytes { get; set; }
    public string FileName { get; set; }
}
