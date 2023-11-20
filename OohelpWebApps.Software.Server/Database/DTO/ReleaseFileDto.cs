using System.ComponentModel.DataAnnotations;

namespace OohelpWebApps.Software.Server.Database.DTO;

public class ReleaseFileDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string Name { get; set; }

    public int Kind { get; set; }

    public int RuntimeVersion { get; set; }

    [Required]
    [MaxLength(36)]
    public string CheckSum { get; set; }

    [Required]
    public int Size { get; set; }

    [Required]
    public DateTime Uploaded { get; set; }

    [MaxLength(128)]
    public string Description { get; set; }

    public Guid ReleaseId { get; set; }

    public ApplicationReleaseDto Release { get; set; }

}
