using System.Diagnostics;
using System.IO;
using OohelpWebApps.Software.Updater.Common;

namespace OohelpWebApps.Software.Updater;
internal class DownloadedUpdate
{
    public ApplicationInfo AppInfo { get; set; }
    public ApplicationRelease Release { get; set; }
    public ReleaseFile ApplicationReleaseFile { get; set; }
    public ReleaseFile ExtractorReleaseFile { get; set; }
    public string ApplicationPascagePath { get; set; }
    public string ExtractorPath { get; set; }
    public bool StartApplicationAfterDeployment { get; set; }
    public void Clear()
    { 
        if(File.Exists(this.ApplicationPascagePath))
            File.Delete(this.ApplicationPascagePath);

        if (File.Exists(this.ExtractorPath))
            File.Delete(this.ExtractorPath);
    }   

    internal void OnApplicationExit(object sender, EventArgs e)
    {
        var executablePath = Process.GetCurrentProcess().MainModule.FileName;
        var extractionPath = System.IO.Path.GetDirectoryName(executablePath);

        var arguments = $"\"{ApplicationPascagePath}\" \"{extractionPath}\" \"{executablePath}\" {(this.StartApplicationAfterDeployment ? "y" : "n")}";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = ExtractorPath,
            UseShellExecute = true,
            Arguments = arguments
        };

        Process.Start(processStartInfo);
    }
    public override string ToString() =>
        $"PascagePath: {this.ApplicationPascagePath}\nExtractorPath: {this.ExtractorPath}\nStartApplicationAfterDeployment: {this.StartApplicationAfterDeployment}";
    
}
