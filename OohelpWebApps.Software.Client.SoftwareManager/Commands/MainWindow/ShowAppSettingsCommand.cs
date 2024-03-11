using SoftwareManager.Services;

namespace SoftwareManager.Commands.MainWindow;
public class ShowAppSettingsCommand : CommandBase
{
    public ShowAppSettingsCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override void Execute(object parameter)
    {
        this.DialogProvider.ShowAppSettingsDialog();
    }
}
