using OohelpWebApps.Software.Domain;
using OohelpWebApps.Software.Server.Database.DTO;

namespace OohelpWebApps.Software.Server.Mapping;
public static class DtoToDomain
{
    public static ApplicationInfo ToDomain(this ApplicationInfoDto dbInfo) =>
        new ApplicationInfo
        {
            Name = dbInfo.Name,
            Id = dbInfo.Id,
            Description = dbInfo.Description,
            IsPublic = dbInfo.IsPublic,
            Releases = dbInfo.Releases?.Count > 0 ? dbInfo.Releases.Select(a => a.ToDomain()).ToList() : new List<ApplicationRelease>(0)
        };

    public static ApplicationRelease ToDomain(this ApplicationReleaseDto dbRelease) =>
        new ApplicationRelease
        {
            Id = dbRelease.Id,
            Kind = (ReleaseKind)dbRelease.Kind,
            ReleaseDate = dbRelease.ReleaseDate,
            Version = new Version(dbRelease.Version),
            ApplicationId = dbRelease.ApplicationId,
            Details = dbRelease.Details?.Count > 0 ? dbRelease.Details.Select(a => a.ToDomain()).ToList() : new List<ReleaseDetail>(0),
            Files = dbRelease.Files?.Count > 0 ? dbRelease.Files.Select(a => a.ToDomain()).ToList() : new List<ReleaseFile>(0)
        };

    public static ReleaseDetail ToDomain(this ReleaseDetailDto dbDetail) =>
        new ReleaseDetail
        {
            Id = dbDetail.Id,
            Description = dbDetail.Description,
            ReleaseId = dbDetail.ReleaseId,
            Kind = (DetailKind)dbDetail.Kind
        };

    public static ReleaseFile ToDomain(this ReleaseFileDto dbFile) =>
        new ReleaseFile
        {
            Id = dbFile.Id,
            Name = dbFile.Name,
            Kind = (FileKind)dbFile.Kind,
            RuntimeVersion = (FileRuntimeVersion)dbFile.RuntimeVersion,
            CheckSum = dbFile.CheckSum,
            Description = dbFile.Description,
            ReleaseId = dbFile.ReleaseId,
            Size = dbFile.Size,
            Uploaded = dbFile.Uploaded
        };
}
