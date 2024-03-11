using System.Windows;
using SoftwareManager.Services;
using SoftwareManager.ViewModels;

namespace SoftwareManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        this.MainWindow = new MainWindow();
        var service = new ApiClientService();
        var dialogsProvider = new DialogsProvider(this.MainWindow);
        var mainViewModel = new MainWindowViewModel(service, dialogsProvider);
        this.MainWindow.DataContext = mainViewModel;
        this.MainWindow.Show();
        MainWindow.Dispatcher.Invoke(service.ReloadDataset);
        base.OnStartup(e);
    }
}
