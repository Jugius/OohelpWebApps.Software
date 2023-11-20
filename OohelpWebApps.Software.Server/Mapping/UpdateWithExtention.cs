using OohelpWebApps.Software.Contracts.Requests;
using OohelpWebApps.Software.Server.Database.DTO;


namespace OohelpWebApps.Software.Server.Mapping;
public static class UpdateWithExtention
{
    public static void UpdateWith(this ApplicationInfoDto dto, ApplicationRequest request)
    {
        dto.Name = request.Name;
        dto.Description = request.Description;
        dto.IsPublic = request.IsPublic;
    }

    public static void UpdateWith(this ApplicationReleaseDto dto, ReleaseRequest request)
    {
        dto.Version = request.Version.ToString();
        dto.Kind = (int)request.Kind;
        dto.ReleaseDate = request.ReleaseDate;
    }

    public static void UpdateWith(this ReleaseDetailDto dto, ReleaseDetailRequest request)
    {
        dto.Kind = (int)request.Kind;
        dto.Description = request.Description;
    }

}
