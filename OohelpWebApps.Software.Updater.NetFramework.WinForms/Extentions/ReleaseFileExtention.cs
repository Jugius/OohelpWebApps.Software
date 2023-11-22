using System.Collections.Generic;
using System.Linq;
using OohelpWebApps.Software.Updater.Common;

namespace OohelpWebApps.Software.Updater.Extentions;
internal static class ReleaseFileExtention
{
    public static ReleaseFile GetMaxByRuntimeVersion(this IEnumerable<ReleaseFile> files)
    { 
        if(files == null || !files.Any()) return null;

        ReleaseFile max = null;
        foreach (var file in files)
        {
            if (max == null || file.RuntimeVersion > max.RuntimeVersion)
            { 
                max = file;
            }            
        }
        return max;
    }
}
