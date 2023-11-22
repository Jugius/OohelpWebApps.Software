using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SoftwareManager.Services;
using SoftwareManager.ViewModels.Entities;
using SoftwareManager.ViewModels.Helpers;

namespace SoftwareManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {       

        private readonly ApiClientService ApplicationsService;
        private readonly Services.DialogsProvider DialogProvider;

        private ApplicationInfoVM selectedApplication;
        private ApplicationReleaseVM selectedRelease;
        public ReadOnlyObservableCollection<ApplicationInfoVM> Applications => ApplicationsService.Applications;
        public ApplicationInfoVM SelectedApplication
        {
            get => selectedApplication;
            set
            {
                selectedApplication = value;
                OnPropertyChanged(nameof(this.SelectedApplication));
            }
        }
        public ApplicationReleaseVM SelectedRelease
        {
            get => selectedRelease;
            set
            {
                selectedRelease = value;
                OnPropertyChanged(nameof(SelectedRelease));
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            this.ApplicationsService = new ApiClientService();
            this.DialogProvider = new DialogsProvider(this);

            this.DataContext = this;
        }

        #region Apllications Commands
        public RelayCommand CreateApplication => _createApplication ??= new RelayCommand(async obj =>
        {
            var newApp = this.DialogProvider.ShowApplicationInfoDialog();
            if (newApp == null) return;

            var res = await ApplicationsService.Create(newApp);

            if (res.IsSuccess)
                this.SelectedApplication = res.Value;
            else
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _createApplication;
        public RelayCommand EditApplication => _editApplication ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationInfoVM appInfo) return;

            var newApp = this.DialogProvider.ShowApplicationInfoDialog(appInfo);
            if (newApp == null) return;

            var res = await ApplicationsService.Edit(newApp);

            if (res.IsSuccess)
                this.SelectedApplication = res.Value;
            else
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _editApplication;
        public RelayCommand RemoveApplication => _removeApplication ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationInfoVM appInfo || !DialogProvider.ShowDeleteQuestion(appInfo)) return;

            var res = await ApplicationsService.Remove(appInfo);

            if (res.IsSuccess)
                this.SelectedApplication = Applications.FirstOrDefault();
            else
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _removeApplication;
        #endregion

        #region Releases Commands
        public RelayCommand CreateRelease => _createRelease ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationInfoVM appInfo) return;

            var release = DialogProvider.ShowApplicationReleaseDialog(appInfo);
            if (release == null) return;

            var res = await ApplicationsService.Create(release);

            if (res.IsSuccess)
                this.SelectedRelease = res.Value;
            else
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _createRelease;
        public RelayCommand EditRelease => _editRelease ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationReleaseVM rel) return;

            var release = DialogProvider.ShowApplicationReleaseDialog(rel);
            if (release == null) return;

            var res = await ApplicationsService.Edit(release);

            if (res.IsSuccess)
                this.SelectedRelease = res.Value;
            else
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _editRelease;
        public RelayCommand RemoveRelease => _removeRelease ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationReleaseVM release || !DialogProvider.ShowDeleteQuestion(release)) return;

            var res = await ApplicationsService.Remove(release);

            if (res.IsSuccess)
                this.SelectedRelease = SelectedApplication?.Releases.FirstOrDefault();
            else
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _removeRelease;
        #endregion

        #region Details Commands


        public RelayCommand AddReleaseDetail => _addReleaseDetail ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationReleaseVM release) return;

            ReleaseDetailVM detail = DialogProvider.ShowReleaseDetailDialog(release);
            if (detail == null) return;

            var res = await ApplicationsService.Create(detail);

            if (!res.IsSuccess)
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");

        });
        private RelayCommand _addReleaseDetail;

        public RelayCommand EditReleaseDetail => _editReleaseDetail ??= new RelayCommand(async obj =>
        {
            if (obj is not ReleaseDetailVM detail) return;

            ReleaseDetailVM updatedDetail = DialogProvider.ShowReleaseDetailDialog(detail);

            if (updatedDetail == null) return;

            var res = await ApplicationsService.Edit(updatedDetail);

            if (!res.IsSuccess)
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _editReleaseDetail;

        public RelayCommand RemoveReleaseDetail => _removeReleaseDetail ??= new RelayCommand(async obj =>
        {
            if (obj is not ReleaseDetailVM detail || !DialogProvider.ShowDeleteQuestion(detail)) return;

            var res = await ApplicationsService.Remove(detail);

            if (!res.IsSuccess)
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _removeReleaseDetail;

        #endregion

        #region Files Commands

        public RelayCommand AddReleaseFile => _addReleaseFile ??= new RelayCommand(async obj =>
        {
            if (obj is not ApplicationReleaseVM release) return;

            var fi = DialogProvider.ShowOpenFileDialod();
            if (fi == null) return;

            ReleaseFileVM releaseFile = new ReleaseFileVM
            {
                Kind = OohelpWebApps.Software.Domain.FileKind.Update,
                RuntimeVersion = OohelpWebApps.Software.Domain.FileRuntimeVersion.Net6,
                Name = fi.Name,
            };
            releaseFile = DialogProvider.ShowReleaseFileDialog(releaseFile);

            if (releaseFile == null) return;

            releaseFile.ReleaseId = release.Id;

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fi.FullName);
            var res = await ApplicationsService.Create(releaseFile, fileBytes);

            if (!res.IsSuccess)
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _addReleaseFile;
        public RelayCommand RemoveReleaseFile => _removeReleaseFile ??= new RelayCommand(async obj =>
        {
            if (obj is not ReleaseFileVM file || !DialogProvider.ShowDeleteQuestion(file)) return;

            var res = await ApplicationsService.Remove(file);

            if (!res.IsSuccess)
                DialogProvider.ShowException(res.Error, "Ошибка записи в базу");
        });
        private RelayCommand _removeReleaseFile;

        public RelayCommand DownloadReleaseFile => _downloadReleaseFile ??= new RelayCommand(async obj =>
        {
            if (obj is not ReleaseFileVM file) return;

            string filePath = this.DialogProvider.ShowSaveAsFileDialog(file.Name);
            if (filePath == null) return;

            try
            {
                await ApplicationsService.DownloadFile(file, filePath);
                this.DialogProvider.ShowFileInExplorer(filePath);
            }
            catch (System.Exception ex)
            {
                DialogProvider.ShowException(ex, "Ошибка загрузки файла");
            }


        });
        private RelayCommand _downloadReleaseFile;

        public RelayCommand OpenInBrowser => _openInBrowser ??= new RelayCommand(obj =>
        {
            if (obj is not ReleaseFileVM file) return;

            string filePath = ApplicationsService.GetDownloadRequestString(file);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
        });
        private RelayCommand _openInBrowser;

        public RelayCommand CopyLinkToClipboard => _copyLinkToClipboard ??= new RelayCommand(obj =>
        {
            if (obj is not Telerik.Windows.Controls.RadGridView radGrid ||
            radGrid.SelectedItem is not ReleaseFileVM file) return;

            string link = ApplicationsService.GetDownloadRequestString(file);

            Clipboard.SetText(link);
            MessageBox.Show("Ссылка скопирована в буфер: " + link);
        });
        private RelayCommand _copyLinkToClipboard;

        public RelayCommand CopyMD5ToClipboard => _сopyMD5ToClipboard ??= new RelayCommand(obj =>
        {
            if (obj is not Telerik.Windows.Controls.RadGridView radGrid ||
            radGrid.SelectedItem is not ReleaseFileVM file) return;


            Clipboard.SetText(file.CheckSum);
            MessageBox.Show("Сумма скопирована в буфер: " + file.CheckSum);
        });
        private RelayCommand _сopyMD5ToClipboard;

        #endregion

        public RelayCommand ReloadDataset => _reloadDataset ??= new RelayCommand(async obj =>
        {

            var res = await ApplicationsService.ReloadDataset();
            if (res.IsSuccess)
                this.SelectedApplication = this.Applications.FirstOrDefault();
            else
                DialogProvider.ShowException(res.Error, "Ошибка загрузки базы");

        });
        private RelayCommand _reloadDataset;
        public RelayCommand SaveJson => _saveJson ??= new RelayCommand(async obj =>
        {
            if (this.Applications.Count == 0) return;
            string file = DialogProvider.ShowSaveAsJsonFileDialog();
            if (file == null) return;

            try
            {
                await ApplicationsService.SaveJson(file);
                this.DialogProvider.ShowFileInExplorer(file);
            }
            catch (System.Exception ex)
            {
                DialogProvider.ShowException(ex.GetBaseException(), "Ошибка API");
            }
        });
        private RelayCommand _saveJson;
        public RelayCommand ShowAppSettings => _showAppSettings ??= new RelayCommand(obj =>
        {
            this.DialogProvider.ShowAppSettingsDialog();
        });
        private RelayCommand _showAppSettings;


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
