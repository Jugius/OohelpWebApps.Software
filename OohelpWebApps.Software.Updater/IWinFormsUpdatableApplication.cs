
namespace OohelpWebApps.Software.Updater;
public interface IWinFormsUpdatableApplication : IUpdatableApplication
{
    Form MainWindow { get; }
}
