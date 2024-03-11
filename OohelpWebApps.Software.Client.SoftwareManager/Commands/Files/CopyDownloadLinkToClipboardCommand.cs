using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Files;
public class CopyDownloadLinkToClipboardCommand : CommandBase
{
    public CopyDownloadLinkToClipboardCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override void Execute(object parameter)
    {
        if (parameter is not Telerik.Windows.Controls.RadGridView radGrid ||
            radGrid.SelectedItem is not ReleaseFileVM file) return;

        string link = ApplicationsService.GetDownloadRequestString(file);

        System.Windows.Clipboard.SetText(link);
        DialogProvider.ShowInformation($"Ссылка скопирована в буфер:\n{link}", file.Name);        
    }
}
