
namespace OohelpWebApps.Software.Updater.WinForms;
internal class ApplicationDeployment(IWinFormsUpdatableApplication application, Dialogs.DialogProvider dialogProvider) : Updater.ApplicationDeployment(application, dialogProvider)
{
    private readonly IWinFormsUpdatableApplication application = application;

    protected override void ShutdownApplication()
    {
        this.application.MainWindow.Invoke(() =>
        {
            System.Windows.Forms.Application.Exit();
        });
    }
}
