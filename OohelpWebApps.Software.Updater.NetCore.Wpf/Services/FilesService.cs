using System.IO;
using System.IO.Packaging;
using System.Security.Cryptography;
using System.Text;
using OohelpWebApps.Software.Updater.Models;

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

    public static async Task ThrowIfChecksumInvalid(FileBytes fileBytes)
    {
        var hash = await ComputeHash(fileBytes.Bytes).ConfigureAwait(false);

        if (!string.Equals(hash, fileBytes.Checksum, StringComparison.OrdinalIgnoreCase))
            throw new Exception($"Ошибка проверки файла {fileBytes.FileName}: не совпадает контрольная сумма");
    }
    private static async Task<string> ComputeHash(byte[] bytes)
    {
        byte[] hashBytes;

        await using (var stream = new MemoryStream(bytes))
        {
            hashBytes = await MD5.Create().ComputeHashAsync(stream);
        }

        var s = new StringBuilder(hashBytes.Length * 2);

        foreach (var b in hashBytes)
            s.Append(b.ToString("X2").ToLower());

        return s.ToString();
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

    internal static async Task<OperationResult<bool>> VerifyChecksum(UpdatePackage pascage)
    {
        var results = await Task.WhenAll(VerifyChecksum(pascage.Application), VerifyChecksum(pascage.Extractor));
        if (results.Any(a => !a.IsSuccess))
        {
            var error = string.Join(Environment.NewLine, results.Where(a => !a.IsSuccess).Select(a => a.Error.Message));
            return new Exception(error);
        }
        return true;
    }
    internal static async Task<OperationResult<bool>> VerifyChecksum(FileBytes fileBytes)
    {
        var hash = await ComputeHash(fileBytes.Bytes).ConfigureAwait(false);
        
        if (string.Equals(hash, fileBytes.Checksum, StringComparison.OrdinalIgnoreCase)) 
            return true;
        
        return new Exception($"{fileBytes.FileName}: не совпадает контрольная сумма");
    }

    internal static async Task<OperationResult<DownloadedUpdate>> SaveFilesToUpdateFolder(DownloadUpdateRequest updateRequest, UpdatePackage pascage)
    {
        var appRes = SaveFileToUpdateFolder(pascage.Application);
        var extrRes = SaveFileToUpdateFolder(pascage.Extractor);
        var results = await Task.WhenAll(appRes, extrRes);

        if (results.All(a => a.IsSuccess))
        {
            return new DownloadedUpdate
            {
                AppInfo = updateRequest.AppInfo,
                Release = updateRequest.Release,
                ApplicationReleaseFile = updateRequest.ApplicationReleaseFile,
                ApplicationPascagePath = appRes.Result.Value,
                ExtractorPath = extrRes.Result.Value,
            };
        }
        
        DeleteFileFromUpdateFolder(pascage.Application);
        DeleteFileFromUpdateFolder(pascage.Extractor);

        var error = string.Join(Environment.NewLine, results.Where(a => !a.IsSuccess).Select(a => a.Error.Message));
        return new Exception(error);
    }
    internal static async Task<OperationResult<string>> SaveFileToUpdateFolder(FileBytes fileBytes)
    {
        string destFolder = Path.Combine(AppContext.BaseDirectory, UPDATE_DIRECTORY_NAME);
        Directory.CreateDirectory(destFolder);
        string destFile = Path.Combine(destFolder, fileBytes.FileName);
        try
        {
            await File.WriteAllBytesAsync(destFile, fileBytes.Bytes);
            return destFile;
        }
        catch (Exception ex)
        {
            return new Exception($"Ошибка сохранения файла {fileBytes.FileName}: {ex.Message}");
        }        
    }
    private static void DeleteFileFromUpdateFolder(FileBytes fileBytes)
    {
        string destFolder = Path.Combine(AppContext.BaseDirectory, UPDATE_DIRECTORY_NAME);        
        string destFile = Path.Combine(destFolder, fileBytes.FileName);

        if (!File.Exists(destFile)) return;

        try
        {
            File.Delete(destFile);
        }
        catch{}

    }
}
