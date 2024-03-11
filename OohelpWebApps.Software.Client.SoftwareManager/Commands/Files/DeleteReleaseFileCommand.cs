using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Files;
public class DeleteReleaseFileCommand : AsyncCommandBase
{
    public DeleteReleaseFileCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ReleaseFileVM file || !DialogProvider.ShowDeleteQuestion(file)) return;

        var res = await ApplicationsService.Remove(file);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
}
