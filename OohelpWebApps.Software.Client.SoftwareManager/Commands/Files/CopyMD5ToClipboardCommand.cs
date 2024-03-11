using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Files;
public class CopyMD5ToClipboardCommand : CommandBase
{
    public CopyMD5ToClipboardCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override void Execute(object parameter)
    {
        if (parameter is not Telerik.Windows.Controls.RadGridView radGrid ||
            radGrid.SelectedItem is not ReleaseFileVM file) return;

        System.Windows.Clipboard.SetText(file.CheckSum);
        DialogProvider.ShowInformation($"Сумма скопирована в буфер:\n{file.CheckSum}", file.Name);
    }
}
