﻿
using OohelpWebApps.Software.Updater.Common;

namespace OohelpWebApps.Software.Updater;
internal class DownloadUpdateRequest : IUpdate
{
    public ApplicationInfo AppInfo { get; set; }
    public ApplicationRelease Release { get; set; }
    public ReleaseFile ApplicationReleaseFile { get; set; }
}
