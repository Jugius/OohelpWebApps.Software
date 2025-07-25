using OohelpWebApps.Software.Updater.Common;

namespace OohelpWebApps.Software.Updater;
internal interface IUpdate
{
    ApplicationInfo AppInfo { get; set; }
    ApplicationRelease Release { get; set; }
    ReleaseFile ApplicationReleaseFile { get; set; }
}
