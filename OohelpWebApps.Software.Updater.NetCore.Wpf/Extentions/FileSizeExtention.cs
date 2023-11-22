
namespace OohelpWebApps.Software.Updater.Extentions;
internal static class FileSizeExtention
{
    private const string MB = " MB";
    private const string KB = " KB";
    private const string GB = " GB";
    public static string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
    {
        double newBytes = bytes;
        string formatString = "{0";
        string byteType;
        if (newBytes > 1024 && newBytes < 1048576)
        {
            newBytes /= 1024;
            byteType = KB;
        }
        else if (newBytes > 1048576 && newBytes < 1073741824)
        {
            newBytes /= 1048576;
            byteType = MB;
        }
        else
        {
            newBytes /= 1073741824;
            byteType = GB;
        }

        if (decimalPlaces > 0)
            formatString += ":0.";

        for (int i = 0; i < decimalPlaces; i++)
        {
            formatString += "0";
        }

        formatString += "}";

        if (showByteType)
            formatString += byteType;

        return string.Format(formatString, newBytes);

    }
}
