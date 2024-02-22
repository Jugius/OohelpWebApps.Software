using System.Windows;

namespace SoftwareManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var mainWindow = new MainWindow();
        this.MainWindow = mainWindow;
        this.MainWindow.Show();
        mainWindow.ReloadDataset.Execute(null);
        base.OnStartup(e);
    }
}
