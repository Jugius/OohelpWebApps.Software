
using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Details;
public class DeleteReleaseDetailCommand : AsyncCommandBase
{
    public DeleteReleaseDetailCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ReleaseDetailVM detail || !DialogProvider.ShowDeleteQuestion(detail)) return;

        var res = await ApplicationsService.Remove(detail);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
