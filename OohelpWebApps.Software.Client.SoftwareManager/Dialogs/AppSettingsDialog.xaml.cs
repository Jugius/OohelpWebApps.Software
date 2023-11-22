using System.ComponentModel;
using System.Windows;


namespace SoftwareManager.Dialogs;
/// <summary>
/// Interaction logic for AppSettingsDialog.xaml
/// </summary>
public partial class AppSettingsDialog : Window, INotifyPropertyChanged
{
    private AppSettings _settings;
    private string username;
    private string password;
    private string authenticationApiServer;
    private string softwareApiServer;

    public AppSettings Settings => _settings;
    public string Username 
    {
        get => username;
        set { username = value; OnPropertyChanged(nameof(Username)); }
    }
    public string Password {
        get => password;
        set { password = value; OnPropertyChanged(nameof(Password)); }
    }
    public string AuthenticationApiServer
    {
        get => authenticationApiServer;
        set { authenticationApiServer = value; OnPropertyChanged(nameof(AuthenticationApiServer)); }
    }
    public string SoftwareApiServer 
    {
        get => softwareApiServer;
        set { softwareApiServer = value; OnPropertyChanged(nameof(SoftwareApiServer)); }
    }

    public AppSettingsDialog(AppSettings settings)
    {
        InitializeComponent();
        this.Username = settings.Username;
        this.Password = settings.Password;
        this.AuthenticationApiServer = settings.AuthenticationApiServer;
        this.SoftwareApiServer = settings.SoftwareApiServer;
        this.DataContext = this;
    }

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
        _settings = new AppSettings
        {
            Username = this.Username,
            Password = this.Password,
            AuthenticationApiServer = this.AuthenticationApiServer,
            SoftwareApiServer = this.SoftwareApiServer
        };
        this.DialogResult = true;
    }
    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}
