namespace OohelpWebApps.Software.Updater.Extentions;
internal static class VersionExtention
{
    public static string ToFormattedString(this Version version) => version.Major + "." + version.Minor +
                            (version.Build > 0 ? $" (build {version.Build}" +
                            (version.Revision > 0 ? $" rev. {version.Revision}" : null) + ")" : null);
}
