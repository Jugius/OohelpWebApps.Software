using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using OohelpWebApps.Software.Updater;

namespace Updater.NetCore.Wpf.UITests;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IUpdatableApplication
{
    private readonly ApplicationDeployment applicationDeployment;    

    public MainWindow()
    {
        InitializeComponent();
        this.applicationDeployment = new ApplicationDeployment(this);
    }

    private readonly Version version = new Version(1, 15, 2);// ApplicationDeployment.GetApplicationVersion(Assembly.GetExecutingAssembly());
    public string ApplicationName => "OohPanel";
    public Version Version => version;
    public Uri UpdatesServer { get; } = new Uri("https://localhost:7164");// new Uri("https://software.oohelp.net");
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
        MessageBox.Show(this.applicationDeployment.ToString(), "Status", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
