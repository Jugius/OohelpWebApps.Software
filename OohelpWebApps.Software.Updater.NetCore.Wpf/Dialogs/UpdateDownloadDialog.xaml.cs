using System.ComponentModel;
using System.Windows;
using OohelpWebApps.Software.Updater.Common;
using OohelpWebApps.Software.Updater.Models;
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

    private string _bytesTotalString = string.Empty;

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
        this._apiService.ContentLengthUpdated += ApiService_ContentLengthUpdated;        
    }

    private void ApiService_ContentLengthUpdated(object sender, long? e)
    {
        if (e.HasValue)
        {
            this._bytesTotalString = FilesService.FormatBytes(e.Value, 1, true);
        }
        else
        {
            this.ProgressIsIndeterminate = true;
        }
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        Progress<DownloadProgress> progress = new Progress<DownloadProgress>(UpdateProgress);
        bool result = false;

        ReleaseFile _appFile = this._updateRequest.ApplicationReleaseFile;

        try
        {
            var downloadResult = await _apiService.DownloadPascage(_appFile, progress, _cancellationTokenSource.Token);
            if (!downloadResult.IsSuccess) throw new Exception($"Ошибка загрузки: {downloadResult.Error.Message}");

            this.ProgressIsIndeterminate = true;
            this.ProgressStatus = "Проверка контрольной суммы...";

            var checkResult = await FilesService.VerifyChecksum(downloadResult.Value);
            if (!checkResult.IsSuccess) throw new Exception($"Ошибка проверки: {checkResult.Error.Message}");

            var saveFilesResult = await FilesService.SaveFilesToUpdateFolder(this._updateRequest, downloadResult.Value);
            if (!saveFilesResult.IsSuccess) throw saveFilesResult.Error;

            this.DownloadedUpdate = saveFilesResult.Value;
            result = true;
        }
        catch (OperationCanceledException)
        {
            this.DownloadedUpdate = null;
            result = true;
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.GetBaseException().Message;
            result = false;
        }
        finally
        {
            this._isFinished = true;
            this._cancellationTokenSource.Dispose();
            this._apiService.ContentLengthUpdated -= ApiService_ContentLengthUpdated;
            this.DialogResult = result;
        }
    }

    private void UpdateProgress(DownloadProgress p)
    {
        var persent = p.GetProgress();
        if (this.ProgressValue != persent)
        {
            this.ProgressValue = persent;
            this.ProgressStatus = $"Завершено: {persent}% ({FilesService.FormatBytes(p.Read, 1, true)} / {_bytesTotalString})";
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
