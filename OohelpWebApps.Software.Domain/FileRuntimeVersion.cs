namespace OohelpWebApps.Software.Domain;

public enum FileRuntimeVersion
{
    NetFramework = 4,
    Net5 = 5,
    Net6 = 6,
    Net7 = 7,
    Net8 = 8,
    Net9 = 9,
    Net10 = 10,
}
public static class FileRuntimeVersionExtentions
{
    public static string ToRuntimeName(this FileRuntimeVersion version) => version switch
    {
        FileRuntimeVersion.NetFramework => ".NET Framework",
        _ => $".NET {(int)version}"
    };
}
