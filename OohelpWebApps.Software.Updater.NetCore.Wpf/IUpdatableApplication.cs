using System.Windows;

namespace OohelpWebApps.Software.Updater;

public interface IUpdatableApplication
{
    string ApplicationName { get; }
    Uri DownloadPage { get; }
    Version Version { get; }
    Uri UpdatesServer { get; }
    Window MainWindow { get; }
}
