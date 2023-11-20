using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Server.Database.DTO;


namespace OohelpWebApps.Software.Server.Mapping;
public static class ContractToDto
{
    public static ApplicationInfoDto ToDto(this ApplicationRequest request) =>
        new ApplicationInfoDto
        {
            Id = Guid.Empty,
            Name = request.Name,
            IsPublic = request.IsPublic,
            Description = request.Description
        };

    public static ApplicationReleaseDto ToDto(this ReleaseRequest request) =>
        new ApplicationReleaseDto
        {
            Id = Guid.Empty,
            Version = request.Version.ToString(),
            Kind = (int)request.Kind,
            ReleaseDate = request.ReleaseDate,
        };

    public static ReleaseDetailDto ToDto(this ReleaseDetailRequest request) =>
        new ReleaseDetailDto
        {
            Id = Guid.Empty,
            Kind = (int)request.Kind,
            Description = request.Description
        };

    public static ReleaseFileDto ToDto(this ReleaseFileRequest request) =>
        new ReleaseFileDto
        {
            Id = Guid.Empty,
            Name = request.Name,
            Kind = (int)request.Kind,
            Description = request.Description,
            RuntimeVersion = (int)request.RuntimeVersion,
            Uploaded = DateTime.Now,
        };
}
