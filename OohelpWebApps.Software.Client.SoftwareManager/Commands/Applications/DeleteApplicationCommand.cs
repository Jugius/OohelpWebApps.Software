using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Applications;
public class DeleteApplicationCommand : AsyncCommandBase
{
    public DeleteApplicationCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationInfoVM appInfo || !DialogProvider.ShowDeleteQuestion(appInfo)) return;

        var res = await ApplicationsService.Remove(appInfo);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
