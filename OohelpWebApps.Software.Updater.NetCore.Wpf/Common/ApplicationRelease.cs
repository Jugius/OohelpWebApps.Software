
using System.Text.Json.Serialization;
using OohelpWebApps.Software.Updater.Common.Enums;

namespace OohelpWebApps.Software.Updater.Common;

internal class ApplicationRelease
{
    public Version Version { get; set; }

    
    [JsonConverter(typeof(JsonStringDateOnlyConverter))]
    public DateOnly ReleaseDate { get; set; }
    public ReleaseKind Kind { get; set; }
    public Guid ApplicationId { get; set; }
    public List<ReleaseDetail> Details { get; set; }
    public List<ReleaseFile> Files { get; set; }
}
