using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Services;

namespace OohelpWebApps.Software.Updater.Dialogs;
internal partial class UpdateDownloadDialog : Form
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private bool _isFinished = false;
    private readonly int _bytesTotal;
    private readonly string _bytesTotalString;
    private int _bytesLoaded;

    private readonly DownloadUpdateRequest updateRequest;
    private readonly ApiSoftwareService apiService;

    public int ProgressValue
    {
        get => this.pbProgress.Value;
        set => this.pbProgress.Value = value;
    }
    public string ProgressStatus
    {
        get => this.lblCurrentProgress.Text;
        set => this.lblCurrentProgress.Text = value;        
    }
    public bool ProgressIsIndeterminate
    {
        get => this.pbProgress.Style == ProgressBarStyle.Marquee;
        set => this.pbProgress.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
    }
    public string CurrentVersion
    {
        get => lblCurrentVersion.Text;
        set => lblCurrentVersion.Text = value;
    }
    public string UpdateVersion
    {
        get => lblUpdateVersion.Text;
        set => lblUpdateVersion.Text = value;
    }
    public string ErrorMessage { get; private set; }
    internal DownloadedUpdate DownloadedUpdate { get; private set; }
    public UpdateDownloadDialog()
    {
        InitializeComponent();
    }
    internal UpdateDownloadDialog(DownloadUpdateRequest updateRequest, ApiSoftwareService apiService) : this()
    {
        this.updateRequest = updateRequest;
        this.apiService = apiService;

        this._bytesTotal = updateRequest.ApplicationReleaseFile.Size + updateRequest.ExtractorReleaseFile.Size;
        this._bytesTotalString = Extentions.FileSizeExtention.FormatBytes(_bytesTotal, 1, true);
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        Progress<int> progress = new Progress<int>(UpdateProgress);
        DialogResult result = DialogResult.None;

        string downloaded_appFile = string.Empty;
        string downloaded_extractorFile = string.Empty;

        ReleaseFile _appFile = this.updateRequest.ApplicationReleaseFile;
        ReleaseFile _extractorFile = this.updateRequest.ExtractorReleaseFile;

        try
        {
            downloaded_appFile = await apiService.DownloadToTempFile(_appFile.Id, progress, _cancellationTokenSource.Token);
            downloaded_extractorFile = await apiService.DownloadToTempFile(_extractorFile.Id, progress, _cancellationTokenSource.Token);

            this.ProgressIsIndeterminate = true;
            this.ProgressStatus = "Проверка контрольной суммы...";

            await Task.WhenAll(
                FilesService.ThrowIfChecksumInvalid(_appFile, downloaded_appFile),
                FilesService.ThrowIfChecksumInvalid(_extractorFile, downloaded_extractorFile));

            downloaded_appFile = FilesService.MoveTempFileToUpdateDirectory(_appFile, downloaded_appFile);
            downloaded_extractorFile = FilesService.MoveTempFileToUpdateDirectory(_extractorFile, downloaded_extractorFile);

            this.DownloadedUpdate = new DownloadedUpdate
            {
                ApplicationReleaseFile = _appFile,
                ApplicationPascagePath = downloaded_appFile,
                ExtractorReleaseFile = _extractorFile,
                ExtractorPath = downloaded_extractorFile,
                Release = this.updateRequest.Release
            };
            result = DialogResult.OK;
        }
        catch (OperationCanceledException)
        {
            ClearFiles(downloaded_appFile, downloaded_extractorFile);
            this.DownloadedUpdate = null;
            result = DialogResult.Cancel;
        }
        catch (Exception ex)
        {
            ClearFiles(downloaded_appFile, downloaded_extractorFile);
            this.ErrorMessage = ex.GetBaseException().Message;
            result = DialogResult.Abort;
        }
        finally
        {
            this._isFinished = true;
            this._cancellationTokenSource.Dispose();
            this.DialogResult = result;
        }
    }



    private static void ClearFiles(params string[] files)
    {
        foreach (var file in files)
        {
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
                System.IO.File.Delete(file);
        }
    }

    private void UpdateProgress(int e)
    {
        this._bytesLoaded += e;
        int persent = this._bytesLoaded * 100 / this._bytesTotal;
        if (this.ProgressValue != persent)
        {
            this.ProgressValue = this._bytesLoaded * 100 / this._bytesTotal;
            this.ProgressStatus = $"Завершено: {persent}% ({Extentions.FileSizeExtention.FormatBytes(this._bytesLoaded, 1, true)} / {_bytesTotalString})";
        }
    }


    private void CancelButton_Click(object sender, System.EventArgs e)
    {
        string message = "Точно прервать загрузку?";
        if (MessageBox.Show(owner: this, message, "Загрузка обновления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            this._cancellationTokenSource.Cancel();
        }
    }
    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_isFinished)
        {
            e.Cancel = true;
            CancelButton_Click(null, null);
        }
        else
        {
            base.OnClosing(e);
        }
    }
}
