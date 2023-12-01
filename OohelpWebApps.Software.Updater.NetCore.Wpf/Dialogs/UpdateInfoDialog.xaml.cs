using System.Windows;

namespace OohelpWebApps.Software.Updater.Dialogs;
/// <summary>
/// Interaction logic for UpdateInfoDialog.xaml
/// </summary>
public partial class UpdateInfoDialog : Window
{
    public UpdateInfoDialog()
    {
        InitializeComponent();
        this.DataContext = this;
    }
    public string ApplicationName { get; set; }
    public string UpdateVersion { get; set; }
    public string UpdateSize { get; set; }
    public string UpdateStatus { get; set; }
    public Uri UpdateDetailsUri { get; set; }
    public string CurrentVersion { get; set; }
    public string LastTimeUpdated { get; set; }
    public string UpdateFeaturesHeader { get; set; }
    public string UpdateDescription { get; set; }
    public string AttentionText => this.ApplicationName + " закроется автоматически.";

    private void Accept_Click(object sender, RoutedEventArgs e)
    {        
        this.DialogResult = true;
    }
}
