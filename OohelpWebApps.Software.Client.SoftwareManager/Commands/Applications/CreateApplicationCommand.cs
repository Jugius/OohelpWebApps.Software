using System.Threading.Tasks;
using SoftwareManager.Services;

namespace SoftwareManager.Commands.Applications;
public class CreateApplicationCommand : AsyncCommandBase
{
    public CreateApplicationCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        var newApp = this.DialogProvider.ShowApplicationInfoDialog();
        if (newApp == null) return;

        var res = await ApplicationsService.Create(newApp);

        if(!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
