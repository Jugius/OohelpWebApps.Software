using System;
using System.Windows.Forms;

namespace OohelpWebApps.Software.Updater;

public interface IUpdatableApplication
{
    string ApplicationName { get; }
    Version Version { get; }
    string UpdatesServerPath { get; }
    Uri DownloadPage { get; }
    Form MainWindow { get; }
}
