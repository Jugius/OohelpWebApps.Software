using System.Collections.ObjectModel;
using System.Linq;

namespace SoftwareManager.Mapping;
public static class DomainToModelView
{
    public static ViewModels.Entities.ApplicationInfoVM ToModelView(this OohelpWebApps.Software.Domain.ApplicationInfo appInfo)
    {
        var modelInfo = new ViewModels.Entities.ApplicationInfoVM
        {
            Id = appInfo.Id,
            Name = appInfo.Name,
            IsPublic = appInfo.IsPublic,
            Description = appInfo.Description
        };
        if (appInfo.Releases != null && appInfo.Releases.Count > 0)
            modelInfo.Releases = new ObservableCollection<ViewModels.Entities.ApplicationReleaseVM>(appInfo.Releases.Select(a => a.ToModelView()).OrderBy(release => release.ReleaseDate));
        return modelInfo;
    }
    public static ViewModels.Entities.ApplicationReleaseVM ToModelView(this OohelpWebApps.Software.Domain.ApplicationRelease apiRelease)
    {
        var modelRelease = new ViewModels.Entities.ApplicationReleaseVM
        {
            ApplicationId = apiRelease.ApplicationId,
            Id = apiRelease.Id,
            Version = apiRelease.Version,
            Kind = apiRelease.Kind,
            ReleaseDate = apiRelease.ReleaseDate
        };
        if (apiRelease.Details != null && apiRelease.Details.Count > 0)
            modelRelease.Details = new ObservableCollection<ViewModels.Entities.ReleaseDetailVM>(apiRelease.Details.Select(a => a.ToModelView()).ToList());

        if (apiRelease.Files != null && apiRelease.Files.Count > 0)
            modelRelease.Files = new ObservableCollection<ViewModels.Entities.ReleaseFileVM>(apiRelease.Files.Select(a => a.ToModelView()).ToList());
        return modelRelease;
    }
    public static ViewModels.Entities.ReleaseDetailVM ToModelView(this OohelpWebApps.Software.Domain.ReleaseDetail apiDetail) =>
         new ViewModels.Entities.ReleaseDetailVM
         {
             Id = apiDetail.Id,
             Description = apiDetail.Description,
             ReleaseId = apiDetail.ReleaseId,
             Kind = apiDetail.Kind
         };

    public static ViewModels.Entities.ReleaseFileVM ToModelView(this OohelpWebApps.Software.Domain.ReleaseFile apiFile) =>
        new ViewModels.Entities.ReleaseFileVM
        {
            Id = apiFile.Id,
            Name = apiFile.Name,
            Kind = apiFile.Kind,
            RuntimeVersion = apiFile.RuntimeVersion,
            CheckSum = apiFile.CheckSum,
            Description = apiFile.Description,
            ReleaseId = apiFile.ReleaseId,
            Size = apiFile.Size,
            Uploaded = apiFile.Uploaded
        };
}
