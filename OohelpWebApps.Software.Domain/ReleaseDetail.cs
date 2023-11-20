namespace OohelpWebApps.Software.Domain;

public class ReleaseDetail
{
    public Guid Id { get; set; }
    public DetailKind Kind { get; set; }
    public string Description { get; set; }
    public Guid ReleaseId { get; set; }
}
