using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Releases;
public class DeleteReleaseCommand : AsyncCommandBase
{
    public DeleteReleaseCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationReleaseVM release || !DialogProvider.ShowDeleteQuestion(release)) return;

        var res = await ApplicationsService.Remove(release);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
