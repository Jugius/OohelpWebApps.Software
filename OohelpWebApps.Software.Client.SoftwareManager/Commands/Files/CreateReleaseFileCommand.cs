using System.Linq;
using System.Threading.Tasks;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Files;
public class CreateReleaseFileCommand : AsyncCommandBase
{

    public CreateReleaseFileCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {        
    }

    public override async Task ExecuteAsync(object parameter)
    {
        if (parameter is not ApplicationReleaseVM release) return;

        var fi = DialogProvider.ShowOpenFileDialod();
        if (fi == null) return;

        ReleaseFileVM releaseFile = new ReleaseFileVM
        {
            Kind = DetectFileKind(fi.Extension),
            RuntimeVersion = System.Enum.GetValues<OohelpWebApps.Software.Domain.FileRuntimeVersion>().Max(),
            Name = fi.Name,
        };
        releaseFile.Description = releaseFile.Kind.ToString();
        releaseFile = DialogProvider.ShowReleaseFileDialog(releaseFile);

        if (releaseFile == null) return;

        releaseFile.ReleaseId = release.Id;

        var fileBytes = await System.IO.File.ReadAllBytesAsync(fi.FullName);
        var res = await ApplicationsService.Create(releaseFile, fileBytes);

        if (!res.IsSuccess)
            DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
    }
    private static OohelpWebApps.Software.Domain.FileKind DetectFileKind(string extension) => extension switch
    {
        ".zip" => OohelpWebApps.Software.Domain.FileKind.Update,
        ".exe" => OohelpWebApps.Software.Domain.FileKind.Install,
        _ => OohelpWebApps.Software.Domain.FileKind.Update,
    };
}
