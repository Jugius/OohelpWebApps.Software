using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Releases;
public class CreateReleaseCommand : AsyncCommandBase
{
    public CreateReleaseCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationInfoVM appInfo) return;

        var release = DialogProvider.ShowApplicationReleaseDialog(appInfo);
        if (release == null) return;

        var res = await ApplicationsService.Create(release);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
