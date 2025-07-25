
namespace OohelpWebApps.Software.Updater.Models;
internal record DownloadProgress(long Total, long Read)
{ 
    public int GetProgress() => (int)Math.Round((double)Read / Total * 100);
}

