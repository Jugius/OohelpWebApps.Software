using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Applications;
public class UpdateApplicationCommand : AsyncCommandBase
{
    public UpdateApplicationCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationInfoVM appInfo) return;

        var newApp = this.DialogProvider.ShowApplicationInfoDialog(appInfo);
        if (newApp == null) return;

        var res = await ApplicationsService.Edit(newApp);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
