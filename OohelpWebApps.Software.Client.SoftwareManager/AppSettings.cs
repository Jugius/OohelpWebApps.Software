using System;
using System.IO;

namespace SoftwareManager;
public class AppSettings
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AuthenticationApiServer { get; set; }
    public string SoftwareApiServer { get; set; }
    public string Token {  get; set; }
    public static class Application
    {
        public static string Name => "SoftwareManager";
        public static string RoamingDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Name;
    }
    public static AppSettings Instance => _instance ??= LoadOrdefault();
    private static AppSettings _instance;
    private static AppSettings LoadOrdefault()
    {
        var file = GetConfigPath();
        if (File.Exists(file))
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(file));
            }
            catch
            {
                return new AppSettings();
            }
        }
        return new AppSettings();
    }
    public static void Save(AppSettings appSettings)
    {
        var filePath = GetConfigPath();
        var jsonString = System.Text.Json.JsonSerializer.Serialize(appSettings, typeof(AppSettings));
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, jsonString);
        _instance = appSettings;
    }
    private static string GetConfigPath()
    {
        var fileName = "appsettings.json";
#if DEBUG
        {
            fileName = "appsettings.Development.json";
        }
#endif
        return Path.Combine(Application.RoamingDirectory, fileName);
    }

}
