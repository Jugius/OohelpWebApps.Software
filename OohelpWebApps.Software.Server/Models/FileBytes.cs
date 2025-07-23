using OohelpWebApps.Software.Domain;

namespace OohelpWebApps.Software.Server.Models;

public class FileBytes
{
    public string ApplicationName { get; set; }
    public string ReleaseVersion { get; set; }
    public byte[] Bytes { get; set; }
    public string FileName { get; set; }
    public string Checksum { get; internal set; }
    public FileKind Kind { get; internal set; }
    public FileRuntimeVersion RuntimeVersion { get; internal set; }
}
