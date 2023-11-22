using System.Windows;

namespace OohelpWebApps.Software.Updater;

public interface IUpdatableApplication
{
    string ApplicationName { get; }
    Uri DownloadPage { get; }
    Version Version { get; }
    string UpdatesServerPath { get; }
    Window MainWindow { get; }
}
