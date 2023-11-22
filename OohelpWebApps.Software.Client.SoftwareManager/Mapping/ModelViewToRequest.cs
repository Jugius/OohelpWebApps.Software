using OohelpWebApps.Software.Contracts.Requests;
using SoftwareManager.ViewModels.Entities;

namespace SoftwareManager.Mapping;
public static class ModelViewToRequest
{
    public static ApplicationRequest ToRequest(this ApplicationInfoVM appInfo) =>
        new ApplicationRequest
        {
            Name = appInfo.Name,
            Description = appInfo.Description,
            IsPublic = appInfo.IsPublic
        };

    public static ReleaseRequest ToRequest(this ApplicationReleaseVM release) =>
        new ReleaseRequest
        {
            Kind = release.Kind,
            ReleaseDate = release.ReleaseDate,
            Version = release.Version
        };

    public static ReleaseDetailRequest ToRequest(this ReleaseDetailVM detail) =>
        new ReleaseDetailRequest
        {
            Kind = detail.Kind,
            Description = detail.Description            
        };

    public static ReleaseFileRequest ToRequest(this ReleaseFileVM file, byte[] fileBytes) =>
        new ReleaseFileRequest
        {
            Name = file.Name,
            Description = file.Description,
            Kind = file.Kind,
            RuntimeVersion = file.RuntimeVersion,
            FileBytes = fileBytes
        };
}
