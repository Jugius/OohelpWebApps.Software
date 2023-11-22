
using System.Collections.Generic;

namespace OohelpWebApps.Software.Updater.Common;
internal class ApplicationInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public List<ApplicationRelease> Releases { get; set; }
}
