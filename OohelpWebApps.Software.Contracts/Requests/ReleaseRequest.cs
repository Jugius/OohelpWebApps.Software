using OohelpWebApps.Software.Domain;

namespace OohelpWebApps.Software.Contracts.Requests;
public class ReleaseRequest
{
    public Version Version { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public ReleaseKind Kind { get; set; }   
}
