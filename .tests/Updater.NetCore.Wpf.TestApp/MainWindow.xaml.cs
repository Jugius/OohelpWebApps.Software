using System;
using System.Threading.Tasks;
using System.Windows;
using OohelpWebApps.Software.Updater;

namespace Updater.NetCore.Wpf.UITests;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, OohelpWebApps.Software.Updater.IUpdatableApplication
{
    private readonly OohelpWebApps.Software.Updater.ApplicationDeployment applicationDeployment;    

    public MainWindow()
    {
        InitializeComponent();
        this.applicationDeployment = new ApplicationDeployment(this);
    }    
    public string ApplicationName => "OohPanel";
    public Version Version => new Version(1, 6);
    public string UpdatesServerPath => @"https://localhost:7164";// @"https://software.oohelp.net";
    Window IUpdatableApplication.MainWindow => this;

    public Uri DownloadPage { get; } = new Uri("https://oohelp.net");

    private void Manual_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(() => this.applicationDeployment.UpdateApplication(UpdateMethod.Manual));
    }

    private void Automatic_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(() => this.applicationDeployment.UpdateApplication(UpdateMethod.Automatic));
    }

    private void AutomaticDownload_UpdateOnRequest_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(() => this.applicationDeployment.UpdateApplication(UpdateMethod.AutomaticDownload_UpdateOnRequest));
    }

    private void DownloadAndUpdateOnRequest_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(() => this.applicationDeployment.UpdateApplication(UpdateMethod.DownloadAndUpdateOnRequest));
    }

    private void GetStatus_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(this.applicationDeployment.GetStatus(), "Status", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void GetRuntime_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(ApplicationDeployment.GetRunningRuntimeVersion().ToString(), "Runtime", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
