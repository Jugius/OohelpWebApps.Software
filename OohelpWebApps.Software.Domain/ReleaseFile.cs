namespace OohelpWebApps.Software.Domain;

public class ReleaseFile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public FileKind Kind { get; set; }
    public FileRuntimeVersion RuntimeVersion { get; set; }
    public string CheckSum { get; set; }
    public int Size { get; set; }
    public DateTime Uploaded { get; set; }
    public string Description { get; set; }
    public Guid ReleaseId { get; set; }
}
