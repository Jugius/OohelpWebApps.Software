using System.Threading.Tasks;
using SoftwareManager.Services;

namespace SoftwareManager.Commands.MainWindow;
public class ReloadDatasetCommand : CommandBase
{
    public ReloadDatasetCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override void Execute(object parameter)
    {
        this.DialogProvider.DialogsOwner.Dispatcher.Invoke(ApplicationsService.ReloadDataset) ;
    }
}
