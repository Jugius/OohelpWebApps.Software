using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Common.Enums;
using OohelpWebApps.Software.Updater.Services;

namespace OohelpWebApps.Software.Updater.Extentions;
internal static class InternalModelsExtention
{
    private static readonly RuntimeVersion runtimeVersion = RuntimeService.Version;
    public static bool HasNewerVersion(this ApplicationInfo application, Version currentAppVersion)
    {
        return application.Releases.Any(r => r.Version > currentAppVersion && r.Files.Count > 0);
    }

    public static bool HasSuitableReleaseToUpdate(this ApplicationInfo application, Version currentAppVersion)
    { 
        return application.Releases
            .Any(release => release.Version > currentAppVersion && release.Files.Any(f => f.RuntimeVersion == runtimeVersion && f.Kind == FileKind.Update));
    }
    public static bool HasSuitableReleaseToInstall(this ApplicationInfo application, Version currentAppVersion)
    {
        return application.Releases
            .Any(release => release.Version > currentAppVersion &&
                release.Files.Any(f => f.RuntimeVersion >= runtimeVersion && f.Kind == FileKind.Install));
    }
    public static ApplicationRelease GetSuitableReleaseToUpdate(this ApplicationInfo application, Version currentAppVersion)
    {
        return application.Releases
            .Where(rel => rel.Version > currentAppVersion &&
                rel.Files.Any(f => f.RuntimeVersion == runtimeVersion && f.Kind == FileKind.Update))
            .MaxBy(a => a.Version);
    }
    public static ApplicationRelease GetSuitableReleaseToInstall(this ApplicationInfo appInfo, Version version)
    {
        return appInfo.Releases
            .Where(rel => rel.Version > version &&
                rel.Files.Any(f => f.RuntimeVersion >= runtimeVersion && f.Kind == FileKind.Install))
            .MaxBy(a => a.Version);
    }

    public static ReleaseFile GetSuitableFileToUpdate(this IEnumerable<ReleaseFile> files) =>
        files.First(f => f.RuntimeVersion == runtimeVersion && f.Kind == FileKind.Update);  
}

