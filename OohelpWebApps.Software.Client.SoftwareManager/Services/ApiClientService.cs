using OohelpSoft.Helpers.Result;
using OohelpWebApps.Software.Domain;
using SoftwareManager.Helpers;
using SoftwareManager.Mapping;
using SoftwareManager.ViewModels.Entities;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoftwareManager.Services
{
    public class ApiClientService
    {
        public event Action ApplicationsLoaded;
        public event Action<ApplicationInfoVM> ApplicationCreated;
        public event Action<ApplicationInfoVM> ApplicationDeleted;
        public event Action<ApplicationReleaseVM> ReleaseCreated;
        public event Action<ApplicationReleaseVM> ReleaseDeleted;

        public event Action<Exception> LoadApplicationsErrorThrown;

        private readonly HttpClient _httpClient;
        private readonly ObservableCollection<ApplicationInfoVM> _applications  = new ObservableCollection<ApplicationInfoVM>();
        private bool _logined = false;
        public ReadOnlyObservableCollection<ApplicationInfoVM> Applications { get; }
        public ApiClientService()
        {
            Applications = new ReadOnlyObservableCollection<ApplicationInfoVM>(_applications);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = GetBaseClientAddress();
        }
        public async Task ReloadDataset()
        {
            var login = await Login();
            if (!login.IsSuccess)
            {
                this.LoadApplicationsErrorThrown?.Invoke(login.Error);
                return;
            }

            try
            {
                var apps = await _httpClient.GetFromJsonAsync<ApplicationInfo[]>("/api/application");
                var views = apps.Select(a => a.ToModelView()).OrderBy(a => a.Name);//.ToArray();
                this._applications.Clear();

                foreach (var application in views)
                    this._applications.Add(application);

                this.ApplicationsLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                this.LoadApplicationsErrorThrown?.Invoke(ex.GetBaseException());
            }            
        }

        private async Task<Result<bool>> Login()
        {
            if (_logined) return true;

            var login = await AuthenticationService.Login();
            if (login.IsSuccess)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login.Value);
                _logined = true;
                return true;
            }
            else
            {
                return login.Error;
            }
        }

        #region Applications CRUD Commands
        internal async Task<Result> Create(ApplicationInfoVM appInfo)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            if (this._applications.Any(a => a.Name.Equals(appInfo.Name, StringComparison.OrdinalIgnoreCase)))
                return new Exception($"Приложение с названием {appInfo.Name} уже существует в базе.");

            var request = appInfo.ToRequest();
            var response = await _httpClient.PostAsJsonAsync("/api/application", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var app = await response.Content.ReadFromJsonAsync<ApplicationInfo>();
            var result = app.ToModelView();
            _applications.Add(result);
            this.ApplicationCreated?.Invoke(result);
            return Result.Success();
        }

        internal async Task<Result<ApplicationInfoVM>> Edit(ApplicationInfoVM appInfo)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var request = appInfo.ToRequest();
            var response = await _httpClient.PutAsJsonAsync($"/api/application/{appInfo.Id}", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var app = await response.Content.ReadFromJsonAsync<ApplicationInfo>();
            var existing = _applications.First(a => a.Id == app.Id);
            existing.UpdatePropertiesBy(app);
            return existing;
        }

        internal async Task<Result> Remove(ApplicationInfoVM appInfo)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var response = await _httpClient.DeleteAsync($"/api/application/{appInfo.Id}");
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var existing = _applications.First(a => a.Id == appInfo.Id);
            _applications.Remove(existing);
            this.ApplicationDeleted?.Invoke(existing);
            return Result.Success();
        }
        #endregion

        #region Releases CRUD Commands
        internal async Task<Result> Create(ApplicationReleaseVM release)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var request = release.ToRequest();
            var response = await _httpClient.PostAsJsonAsync($"/api/release/{release.ApplicationId}", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var newRelease = await response.Content.ReadFromJsonAsync<ApplicationRelease>();
            var result = newRelease.ToModelView();
            var app = _applications.First(a => a.Id == result.ApplicationId);
            app.Releases.Add(result);
            this.ReleaseCreated?.Invoke(result);
            return Result.Success(); ;
        }

        internal async Task<Result> Edit(ApplicationReleaseVM release)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var request = release.ToRequest();
            var response = await _httpClient.PutAsJsonAsync($"/api/release/{release.Id}", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var result = await response.Content.ReadFromJsonAsync<ApplicationRelease>();
            var existing = _applications.First(a => a.Id == result.ApplicationId)
                    .Releases.First(a => a.Id == result.Id);
            existing.UpdatePropertiesBy(result);
            return Result.Success();
        }

        internal async Task<Result> Remove(ApplicationReleaseVM release)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var response = await _httpClient.DeleteAsync($"/api/release/{release.Id}");
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var app = _applications.First(a => a.Id == release.ApplicationId);
            var existing = app.Releases.First(a => a.Id == release.Id);
            app.Releases.Remove(existing);
            this.ReleaseDeleted?.Invoke(existing);
            return Result.Success();
        }
        #endregion

        #region Details CRUD Commands
        internal async Task<Result<ReleaseDetailVM>> Create(ReleaseDetailVM detail)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var request = detail.ToRequest();

            var response = await _httpClient.PostAsJsonAsync($"/api/detail/{detail.ReleaseId}", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var newDetail = await response.Content.ReadFromJsonAsync<ReleaseDetail>();
            var result = newDetail.ToModelView();
            var release = _applications.SelectMany(a => a.Releases).First(a => a.Id == result.ReleaseId);
            release.Details.Add(result);
            return result;
        }
        internal async Task<Result<ReleaseDetailVM>> Edit(ReleaseDetailVM detail)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var request = detail.ToRequest();
            var response = await _httpClient.PutAsJsonAsync($"/api/detail/{detail.Id}", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var result = await response.Content.ReadFromJsonAsync<ReleaseDetail>();
            var existing = _applications.SelectMany(a => a.Releases)
                    .First(a => a.Id == result.ReleaseId)
                    .Details.First(a => a.Id == result.Id);
            existing.UpdatePropertiesBy(result);
            return existing;
        }
        internal async Task<Result<bool>> Remove(ReleaseDetailVM detail)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var response = await _httpClient.DeleteAsync($"/api/detail/{detail.Id}");
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var release = _applications
                .SelectMany(a => a.Releases)
                .First(a => a.Id == detail.ReleaseId);

            var d = release.Details.First(a => a.Id == detail.Id);
            release.Details.Remove(d);
            return true;
        }
        #endregion

        #region Files CRUD Commands
        internal async Task<Result<ReleaseFileVM>> Create(ReleaseFileVM releaseFile, byte[] fileBytes)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;
            
            var request = releaseFile.ToRequest(fileBytes);
            var response = await _httpClient.PostAsJsonAsync($"/api/file/{releaseFile.ReleaseId}", request);
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();


            var newFile = await response.Content.ReadFromJsonAsync<ReleaseFile>();
            var result = newFile.ToModelView();
            var release = _applications.SelectMany(a => a.Releases).First(a => a.Id == result.ReleaseId);
            release.Files.Add(result);
            return result;
        }
        internal async Task<Result<bool>> Remove(ReleaseFileVM releaseFile)
        {
            var login = await Login();
            if (!login.IsSuccess) return login.Error;

            var response = await _httpClient.DeleteAsync($"/api/file/{releaseFile.Id}");
            if (!response.IsSuccessStatusCode) return await response.ToHttpException();

            var release = _applications.SelectMany(a => a.Releases)
                    .First(a => a.Id == releaseFile.ReleaseId);
            release.Files.Remove(releaseFile);
            return true;
        }

        internal async Task DownloadFile(ReleaseFileVM file, string filePath)
        {
            string path = GetDownloadRequestString(file);
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(path))
                using (var fs = new FileStream(filePath, FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }
        #endregion

        private static Uri GetBaseClientAddress()
        {
            string server = AppSettings.Instance.SoftwareApiServer;
            return string.IsNullOrEmpty(server) ? null : new Uri($"https://{server}");
        }

        public string GetDownloadRequestString(ReleaseFileVM file)
        {
            string server = AppSettings.Instance.SoftwareApiServer;
            return string.IsNullOrEmpty(server) ? null : $"https://{server}/api/file/{file.Id}";
        }
    }
}
