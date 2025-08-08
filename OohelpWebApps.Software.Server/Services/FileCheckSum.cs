using System.Text;

namespace OohelpWebApps.Software.Server.Services;

public class FileCheckSum
{
    public static async Task<string> GetCheckSum(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var crypto = System.Security.Cryptography.MD5.Create();
        byte[] hash = await crypto.ComputeHashAsync(stream);

        StringBuilder s = new StringBuilder(hash.Length * 2);

        foreach (byte b in hash)
            s.Append(b.ToString("X2").ToLower());

        return s.ToString();
    }
}
