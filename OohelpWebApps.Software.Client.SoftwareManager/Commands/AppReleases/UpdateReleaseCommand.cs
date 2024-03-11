using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Releases;
public class UpdateReleaseCommand : AsyncCommandBase
{
    public UpdateReleaseCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationReleaseVM rel) return;

        var release = DialogProvider.ShowApplicationReleaseDialog(rel);
        if (release == null) return;

        var res = await ApplicationsService.Edit(release);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
