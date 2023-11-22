using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SoftwareManager.Services;

public static class Hasher
{
    public enum HashType
    {
        MD5, SHA1, SHA512
    }

    public static string GetHash(byte[] bytes, HashType hashType)
    {
        using var stream = new MemoryStream(bytes);
        return hashType switch
        {
            HashType.MD5 => MakeHashString(MD5.Create().ComputeHash(stream)),
            HashType.SHA1 => MakeHashString(SHA1.Create().ComputeHash(stream)),
            HashType.SHA512 => MakeHashString(SHA512.Create().ComputeHash(stream)),
            _ => "",
        };
    }

    public static string GetHash(FileInfo file, HashType hashType)
    {
        using var stream = file.OpenRead();
        return hashType switch
        {
            HashType.MD5 => MakeHashString(MD5.Create().ComputeHash(stream)),
            HashType.SHA1 => MakeHashString(SHA1.Create().ComputeHash(stream)),
            HashType.SHA512 => MakeHashString(SHA512.Create().ComputeHash(stream)),
            _ => "",
        };

    }
    private static string MakeHashString(byte[] hash)
    {
        StringBuilder s = new StringBuilder(hash.Length * 2);

        foreach (byte b in hash)
            s.Append(b.ToString("X2").ToLower());

        return s.ToString();
    }
}
