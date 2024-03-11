﻿using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Commands.Files;
public class OpenDownloadLinkInBrowserCommand : CommandBase
{
    public OpenDownloadLinkInBrowserCommand(ApiClientService applicationsService, DialogsProvider dialogProvider) : base(applicationsService, dialogProvider)
    {
    }

    public override void Execute(object parameter)
    {
        if (parameter is not ReleaseFileVM file) return;

        string filePath = ApplicationsService.GetDownloadRequestString(file);
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
    }
}
