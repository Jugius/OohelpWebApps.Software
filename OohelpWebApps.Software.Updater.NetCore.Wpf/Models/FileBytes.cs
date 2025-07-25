using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Models;

internal class FileBytes
{
    public string ApplicationName { get; set; }
    public string ReleaseVersion { get; set; }
    public byte[] Bytes { get; set; }
    public string FileName { get; set; }
    public string Checksum { get; set; }
}
