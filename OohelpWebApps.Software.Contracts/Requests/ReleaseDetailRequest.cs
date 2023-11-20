using OohelpWebApps.Software.Domain;

namespace OohelpWebApps.Software.Contracts.Requests;
public class ReleaseDetailRequest 
{
    public DetailKind Kind { get; set; }
    public string Description { get; set; }
}
