using System.ComponentModel;
using System.Windows;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Services;

namespace OohelpWebApps.Software.Updater.Dialogs;
/// <summary>
/// Interaction logic for UpdateDownloadDialog.xaml
/// </summary>
internal partial class UpdateDownloadDialog : Window, INotifyPropertyChanged
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private bool _isFinished = false;

    private readonly ApiSoftwareService _apiService;
    private readonly DownloadUpdateRequest _updateRequest;


    private int progressValue;
    private string progressStatus;
    private bool progressIsIndeterminate;

    private readonly int _bytesTotal;
    private readonly string _bytesTotalString;
    private int _bytesLoaded;
    internal DownloadedUpdate DownloadedUpdate { get; private set; }
    internal string ErrorMessage { get; private set; }

    public int ProgressValue
    {
        get => progressValue;
        set { progressValue = value; OnPropertyChanged(nameof(ProgressValue)); }
    }
    public string ProgressStatus
    {
        get => progressStatus;
        set { progressStatus = value; OnPropertyChanged(nameof(ProgressStatus)); }
    }
    public bool ProgressIsIndeterminate
    {
        get => progressIsIndeterminate;
        set { progressIsIndeterminate = value; OnPropertyChanged(nameof(ProgressIsIndeterminate)); }
    }
    public string CurrentVersion { get; set; }
    public string UpdateVersion { get; set; }
    public UpdateDownloadDialog()
    {
        InitializeComponent();
        this.DataContext = this;
    }
    internal UpdateDownloadDialog(DownloadUpdateRequest updateRequest, ApiSoftwareService apiService) : this()
    {
        this._apiService = apiService;
        this._updateRequest = updateRequest;

        this._bytesTotal = updateRequest.ApplicationReleaseFile.Size + updateRequest.ExtractorReleaseFile.Size;
        this._bytesTotalString = FilesService.FormatBytes(_bytesTotal, 1, true);
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        Progress<int> progress = new Progress<int>(UpdateProgress);
        bool result = false;

        string downloaded_appFile = string.Empty;
        string downloaded_extractorFile = string.Empty;

        ReleaseFile _appFile = this._updateRequest.ApplicationReleaseFile;
        ReleaseFile _extractorFile = this._updateRequest.ExtractorReleaseFile;

        try
        {
            downloaded_appFile = await _apiService.DownloadToTempFile(_appFile.Id, progress, _cancellationTokenSource.Token);
            downloaded_extractorFile = await _apiService.DownloadToTempFile(_extractorFile.Id, progress, _cancellationTokenSource.Token);

            this.ProgressIsIndeterminate = true;
            this.ProgressStatus = "Проверка контрольной суммы...";                        

            await Task.WhenAll(
                FilesService.ThrowIfChecksumInvalid(_appFile, downloaded_appFile),
                FilesService.ThrowIfChecksumInvalid(_extractorFile, downloaded_extractorFile));

            downloaded_appFile = FilesService.MoveFileToUpdateDirectory(downloaded_appFile, _appFile.Name);
            downloaded_extractorFile = FilesService.MoveFileToUpdateDirectory(downloaded_extractorFile, _extractorFile.Name);

            this.DownloadedUpdate = new DownloadedUpdate
            {
                ApplicationReleaseFile = _appFile,
                ApplicationPascagePath = downloaded_appFile,
                ExtractorReleaseFile = _extractorFile,
                ExtractorPath = downloaded_extractorFile,
                Release = this._updateRequest.Release
            };
            result = true;
        }
        catch (OperationCanceledException)
        {
            ClearFiles(downloaded_appFile, downloaded_extractorFile);
            this.DownloadedUpdate = null;
            result = true;
        }
        catch (Exception ex)
        {
            ClearFiles(downloaded_appFile, downloaded_extractorFile);
            this.ErrorMessage = ex.GetBaseException().Message;
            result = false;
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
            if(!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
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
            this.ProgressStatus = $"Завершено: {persent}% ({FilesService.FormatBytes(this._bytesLoaded, 1, true)} / {_bytesTotalString})";
        }
    }



    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        string message = "Точно прервать загрузку?";
        if (MessageBox.Show(owner: this, message, "Загрузка обновления", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
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
