using System.IO;
using System.Security.Cryptography;
using System.Text;
using OohelpWebApps.Software.Updater.Common;

namespace OohelpWebApps.Software.Updater.Services;
internal static class FilesService
{
    const string UPDATE_DIRECTORY_NAME = "updates";

    const string KB = " KB";
    const string MB = " MB";
    const string GB = " GB";
    const int KBValue = 1024;
    const int MBValue = 1048576;
    const int GBValue = 1073741824;

    public static async Task<string> ComputeFileHash(string filePath)
    {
        byte[] bytes;

        await using (var stream = System.IO.File.OpenRead(filePath))
        {
            bytes = await MD5.Create().ComputeHashAsync(stream);
        }

        var s = new StringBuilder(bytes.Length * 2);

        foreach (var b in bytes)
            s.Append(b.ToString("X2").ToLower());

        return s.ToString();
    }
    public static async Task ThrowIfChecksumInvalid(ReleaseFile releaseFile, string filePath)
    {
        var hash = await ComputeFileHash(filePath).ConfigureAwait(false);
        
        if (!string.Equals(hash, releaseFile.CheckSum, StringComparison.OrdinalIgnoreCase))
            throw new Exception($"Ошибка проверки файла {releaseFile.Name}: не совпадает контрольная сумма");
    }
    public static async Task<bool> VerifyChecksum(string filePath, string checkSum)
    {
        var fileHash = await ComputeFileHash(filePath).ConfigureAwait(false);
        return string.Equals(fileHash, checkSum, StringComparison.OrdinalIgnoreCase);
    }
    public static string MoveFileToUpdateDirectory(string sourceFile, string destinationFileName)
    {
        string destFolder = Path.Combine(AppContext.BaseDirectory, UPDATE_DIRECTORY_NAME);
        Directory.CreateDirectory(destFolder);
        string destFile = Path.Combine(destFolder, destinationFileName);
        File.Move(sourceFile, destFile, true);
        return destFile;
    }
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
