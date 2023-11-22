using System;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Common;

internal class ReleaseFile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public FileKind Kind { get; set; }
    public RuntimeVersion RuntimeVersion { get; set; }
    public string CheckSum { get; set; }
    public int Size { get; set; }
    public DateTime Uploaded { get; set; }
    public string Description { get; set; }    
}
