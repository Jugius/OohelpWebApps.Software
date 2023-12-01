using System;
using System.Collections.Generic;
using System.Linq;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Extentions;
internal static class InternalModelsExtention
{
    public static bool HasNewerVersion(this ApplicationInfo application, Version version)
    {
        return application.Releases.Any(r => r.Version > version && r.Files.Any());
    }
    public static bool TryGetSuitableReleaseToUpdate(this ApplicationInfo appInfo, Version version, RuntimeVersion runtimeVersion, out ApplicationRelease release)
    {
        release = appInfo.Releases
            .Where(rel => rel.Version > version &&
                rel.Files.Any(f => f.RuntimeVersion == runtimeVersion && f.Kind == FileKind.Update))
            .OrderByDescending(a => a.Version)
            .FirstOrDefault();

        return release != null;
    }
    public static bool TryGetSuitableReleaseToInstall(this ApplicationInfo appInfo, Version version, RuntimeVersion runtimeVersion, out ApplicationRelease release)
    {
        release = appInfo.Releases
                .Where(rel => rel.Version > version &&
                 rel.Files.Any(f => f.RuntimeVersion > runtimeVersion && f.Kind == FileKind.Install))
                .OrderByDescending(a => a.Version)
                .FirstOrDefault();

        return release != null;
    }
    public static string ToValueString(this DetailKind kind) => kind switch
    {
        DetailKind.Changed => "Изменения:",
        DetailKind.Fixed => "Исправления:",
        DetailKind.Updated => "Обновления:",
        _ => kind.ToString()
    };
    public static ReleaseFile GetSuitableFileToUpdate(this IEnumerable<ReleaseFile> files, RuntimeVersion runtimeVersion) =>
        files.First(f => f.RuntimeVersion == runtimeVersion && f.Kind == FileKind.Update);

    public static ReleaseFile GetSuitableFileToInstall(this IEnumerable<ReleaseFile> files, RuntimeVersion runtimeVersion) =>
        files.First(f => f.RuntimeVersion > runtimeVersion && f.Kind == FileKind.Install);


}

