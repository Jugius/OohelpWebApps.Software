
using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Details;
public class UpdateReleaseDetailCommand : AsyncCommandBase
{
    public UpdateReleaseDetailCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ReleaseDetailVM detail) return;

        ReleaseDetailVM updatedDetail = DialogProvider.ShowReleaseDetailDialog(detail);

        if (updatedDetail == null) return;

        var res = await ApplicationsService.Edit(updatedDetail);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
