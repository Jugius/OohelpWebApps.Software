namespace OohelpWebApps.Software.Updater;

[Flags]
internal enum UpdateStatus
{
    None = 1,
    Downloading = 2,
    Extracted = 4,
    WaitingApplicationExit = 8
}
