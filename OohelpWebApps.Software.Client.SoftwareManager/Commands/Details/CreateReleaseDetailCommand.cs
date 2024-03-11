using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Details;
public class CreateReleaseDetailCommand : AsyncCommandBase
{
    public CreateReleaseDetailCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationReleaseVM release) return;

        ReleaseDetailVM detail = DialogProvider.ShowReleaseDetailDialog(release);
        if (detail == null) return;

        var res = await ApplicationsService.Create(detail);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
