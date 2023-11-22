
using System;
using System.Collections.Generic;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Common;

internal class ApplicationRelease
{
    public Guid Id { get; set; }
    public Version Version { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ReleaseKind Kind { get; set; }    
    public List<ReleaseFile> Files { get; set; }
    public List<ReleaseDetail> Details { get; set; }
}
