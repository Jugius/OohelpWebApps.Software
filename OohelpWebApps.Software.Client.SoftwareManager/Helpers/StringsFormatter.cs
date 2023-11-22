using System.Text;

namespace SoftwareManager.Helpers;

internal static class StringsFormatter
{
    const int KBValue = 1024;
    const int MBValue = 1048576;
    const int GBValue = 1073741824;
    public static string FormatBytes(int bytes, int decimalPlaces, bool showByteType)
    {
        double newBytes = bytes;
        StringBuilder formatString = new StringBuilder("{0");

        if (decimalPlaces > 0)
            formatString.Append(":0.");

        for (int i = 0; i < decimalPlaces; i++)
            formatString.Append('0');

        formatString.Append('}');

        string byteType;
        if (newBytes > KBValue && newBytes < MBValue)
        {
            newBytes /= KBValue;
            byteType = "KB";
        }
        else if (newBytes > MBValue && newBytes < GBValue)
        {
            newBytes /= MBValue;
            byteType = "MB";
        }
        else
        {
            newBytes /= GBValue;
            byteType = "GB";
        }        

        if (showByteType)
            formatString.Append(byteType);

        return string.Format(formatString.ToString(), newBytes);
    }
}
