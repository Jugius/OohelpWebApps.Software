using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Services;

internal static class RuntimeService
{
    private static readonly RuntimeVersion runtimeVersion = GetRuntimeVersion();
    public static RuntimeVersion Version => runtimeVersion;
    private static RuntimeVersion GetRuntimeVersion()
    {
        var netVer = Environment.Version;

        int ver = netVer.Major * 10 + netVer.Minor;

        if (ver >= 100) return RuntimeVersion.Net10;
        if (ver >= 90) return RuntimeVersion.Net9;
        if (ver >= 80) return RuntimeVersion.Net8;

        if (ver >= 50) throw new Exception($"Unsupported runtime version: {ver}");

        return RuntimeVersion.NetFramework;
    }
}
