namespace OohelpWebApps.Software.Domain;

public class ApplicationRelease
{
    public Guid Id { get; set; }
    public Version Version { get; set; }


    public DateOnly ReleaseDate { get; set; }
    public ReleaseKind Kind { get; set; }
    public Guid ApplicationId { get; set; }
    public List<ReleaseDetail> Details { get; set; }
    public List<ReleaseFile> Files { get; set; }
}
