using System.Text.Json.Serialization;
using OohelpWebApps.Software.Domain;

namespace OohelpWebApps.Software.Server.ObsoleteRequests;
public class GetLastReleaseRequest
{
    public string Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FileRuntimeVersion? RuntimeVersion { get; set; }
}
