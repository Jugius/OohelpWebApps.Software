using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OohelpWebApps.Software.Updater.Common;

namespace OohelpWebApps.Software.Updater.Services;
internal static class FilesService
{
    public static async Task<string> ComputeFileHash(string filePath)
    {
        var task = Task.Run(() => 
        {
            byte[] bytes;

            using (var stream = System.IO.File.OpenRead(filePath))
            {
                bytes = MD5.Create().ComputeHash(stream);
            }

            var s = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
                s.Append(b.ToString("X2").ToLower());

            return s.ToString();

        });
        return await task.ConfigureAwait(false);
    }
    public static async Task ThrowIfChecksumInvalid(ReleaseFile releaseFile, string filePath)
    {
        var hash = await ComputeFileHash(filePath).ConfigureAwait(false);
        
        if (!string.Equals(hash, releaseFile.CheckSum, StringComparison.OrdinalIgnoreCase))
            throw new Exception($"Ошибка проверки файла {releaseFile.Name}: не совпадает контрольная сумма");
    }
    public static string MoveTempFileToUpdateDirectory(ReleaseFile releaseFile, string filePath)
    {
        const string UPDATE_DIRECTORY_NAME = "updates";
        string destFolder = Path.Combine(AppContext.BaseDirectory, UPDATE_DIRECTORY_NAME);
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);

        try
        {
            string destFile = Path.Combine(destFolder, releaseFile.Name);

            if (File.Exists(destFile))
                File.Delete(destFile);

            File.Move(filePath, destFile);

            return destFile;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка переноса файла {releaseFile.Name}: " + ex.GetBaseException().Message);
        }
    }
}
