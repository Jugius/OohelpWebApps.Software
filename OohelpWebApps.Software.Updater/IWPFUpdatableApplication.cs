using System.Windows;

namespace OohelpWebApps.Software.Updater;
public interface IWPFUpdatableApplication : IUpdatableApplication
{
    Window MainWindow { get; }
}
