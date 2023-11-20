using System.ComponentModel.DataAnnotations;

namespace OohelpWebApps.Software.Server.Database.DTO;

public class ReleaseDetailDto
{
    public Guid Id { get; set; }
    public int Kind { get; set; }


    [MaxLength(256)]
    public string Description { get; set; }

    public Guid ReleaseId { get; set; }
    public ApplicationReleaseDto Release { get; set; }
}
