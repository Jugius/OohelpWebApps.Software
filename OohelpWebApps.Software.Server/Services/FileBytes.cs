namespace OohelpWebApps.Software.Server.Services;

public class FileBytes
{
    public FileBytes(byte[] bytes, string fileName)
    {
        Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
    }

    public byte[] Bytes { get; }
    public string FileName { get; }
}
