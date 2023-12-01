
namespace SoftwareManager.Helpers;

internal static class StringsFormatter
{
    const string KB = " KB";
    const string MB = " MB";
    const string GB = " GB";
    const int KBValue = 1024;
    const int MBValue = 1048576;
    const int GBValue = 1073741824;
    public static string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
    {
        double newBytes = bytes;
        string byteType;
        if (newBytes > KBValue && newBytes < MBValue)
        {
            newBytes /= KBValue;
            byteType = KB;
        }
        else if (newBytes > MBValue && newBytes < GBValue)
        {
            newBytes /= MBValue;
            byteType = MB;
        }
        else
        {
            newBytes /= GBValue;
            byteType = GB;
        }

        string formatString = BuildFormatString(decimalPlaces);
        if (showByteType)
            formatString += byteType;

        return string.Format(formatString, newBytes);
    }
    private static string BuildFormatString(int decimalPlaces)
    {
        if (decimalPlaces <= 0) return "{0}";
        char[] chars = new char[decimalPlaces + 6];
        chars[0] = '{';
        chars[1] = '0';
        chars[2] = ':';
        chars[3] = '0';
        chars[4] = '.';
        int currentCharIndex = 5;
        for (int i = 0; i < decimalPlaces; i++)
        {
            chars[currentCharIndex++] = '0';
        }
        chars[^1] = '}';
        return new string(chars);
    }
}
