using OohelpWebApps.Software.Domain;

namespace OohelpWebApps.Software.Contracts.Requests;
public class ReleaseFileRequest
{    
    public string Name { get; set; }
    public FileKind Kind { get; set; }
    public FileRuntimeVersion RuntimeVersion { get; set; }
    public string Description { get; set; }
    public byte[] FileBytes { get; set; }
}
