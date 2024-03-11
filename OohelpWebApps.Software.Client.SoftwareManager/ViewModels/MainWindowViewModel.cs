using System;
using System.Collections.ObjectModel;
using System.Linq;
using SoftwareManager.Commands.Applications;
using SoftwareManager.Commands.Details;
using SoftwareManager.Commands.Files;
using SoftwareManager.Commands.MainWindow;
using SoftwareManager.Commands.Releases;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private readonly ApiClientService ApplicationsService;
    private readonly DialogsProvider DialogProvider;

    private ApplicationInfoVM selectedApplication;
    private ApplicationReleaseVM selectedRelease; 

    public ReadOnlyObservableCollection<ApplicationInfoVM> Applications => ApplicationsService.Applications;
    public ApplicationInfoVM SelectedApplication
    {
        get => selectedApplication;
        set => Set(ref selectedApplication, value);
    }
    public ApplicationReleaseVM SelectedRelease
    {
        get => selectedRelease;
        set => Set(ref selectedRelease, value);
    }
    public MainWindowViewModel(ApiClientService applicationsService, DialogsProvider dialogProvider)
    {
        ApplicationsService = applicationsService ?? throw new ArgumentNullException(nameof(applicationsService));
        DialogProvider = dialogProvider ?? throw new ArgumentNullException(nameof(dialogProvider));

        this.ApplicationsService.ApplicationCreated += (app) => { this.SelectedApplication = app; };
        this.ApplicationsService.ApplicationDeleted += (app) => { this.SelectedApplication = ApplicationsService.Applications.FirstOrDefault(); };
        this.ApplicationsService.ReleaseCreated += (r) => { this.SelectedRelease = r; };
        this.ApplicationsService.ReleaseDeleted += (r) => { this.SelectedRelease = this.SelectedApplication?.Releases.MaxBy(a => a.Version); };
        this.ApplicationsService.LoadApplicationsErrorThrown += (ex) => { this.DialogProvider.ShowException(ex, "Ошибка загрузки базы"); };
        this.ApplicationsService.ApplicationsLoaded += () => { this.SelectedApplication = this.Applications.FirstOrDefault(); };

        this.CreateApplication = new CreateApplicationCommand(ApplicationsService, DialogProvider);
        this.EditApplication = new UpdateApplicationCommand(ApplicationsService, DialogProvider);
        this.RemoveApplication = new DeleteApplicationCommand(ApplicationsService, DialogProvider);

        this.CreateRelease = new CreateReleaseCommand(ApplicationsService, DialogProvider);
        this.EditRelease = new UpdateReleaseCommand(ApplicationsService, DialogProvider);
        this.RemoveRelease = new DeleteReleaseCommand(ApplicationsService, DialogProvider);

        this.AddReleaseDetail = new CreateReleaseDetailCommand(ApplicationsService, DialogProvider);
        this.EditReleaseDetail = new UpdateReleaseDetailCommand(ApplicationsService, DialogProvider);
        this.RemoveReleaseDetail = new DeleteReleaseDetailCommand(ApplicationsService, DialogProvider);

        this.AddReleaseFile = new CreateReleaseFileCommand(ApplicationsService, DialogProvider);
        this.RemoveReleaseFile = new DeleteReleaseFileCommand(ApplicationsService, DialogProvider);
        this.DownloadReleaseFile = new DownloadReleaseFileCommand(ApplicationsService, DialogProvider);
        this.OpenInBrowser = new OpenDownloadLinkInBrowserCommand(ApplicationsService, DialogProvider);
        this.CopyLinkToClipboard = new CopyDownloadLinkToClipboardCommand(ApplicationsService, DialogProvider);
        this.CopyMD5ToClipboard = new CopyMD5ToClipboardCommand(ApplicationsService, DialogProvider);

        this.ReloadDataset = new ReloadDatasetCommand(ApplicationsService, DialogProvider);
        this.ShowAppSettings = new ShowAppSettingsCommand(ApplicationsService, DialogProvider);
    }

    //Apllications Commands
    public CreateApplicationCommand CreateApplication { get; }
    public UpdateApplicationCommand EditApplication { get; }
    public DeleteApplicationCommand RemoveApplication { get; }


    //Releases Commands
    public CreateReleaseCommand CreateRelease { get; }
    public UpdateReleaseCommand EditRelease { get; }
    public DeleteReleaseCommand RemoveRelease { get; }


    //Details Commands
    public CreateReleaseDetailCommand AddReleaseDetail { get; }
    public UpdateReleaseDetailCommand EditReleaseDetail { get; }
    public DeleteReleaseDetailCommand RemoveReleaseDetail { get; }


    //Files Commands
    public CreateReleaseFileCommand AddReleaseFile { get; }
    public DeleteReleaseFileCommand RemoveReleaseFile { get; }
    public DownloadReleaseFileCommand DownloadReleaseFile { get; }
    public OpenDownloadLinkInBrowserCommand OpenInBrowser { get; }
    public CopyDownloadLinkToClipboardCommand CopyLinkToClipboard { get; }
    public CopyMD5ToClipboardCommand CopyMD5ToClipboard { get; }


    //Main Window Commands
    public ReloadDatasetCommand ReloadDataset { get; }
    public ShowAppSettingsCommand ShowAppSettings { get; }
}
