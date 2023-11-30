using System.Reflection;
using System.Text;

namespace OohelpWebApps.Software.ZipExtractor.NetCore.WinForms;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static async Task Main()
    {
        StringBuilder _logBuilder = new StringBuilder();
        _logBuilder.AppendLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
        _logBuilder.AppendLine($"Runtime: {Environment.Version}");
        _logBuilder.AppendLine(DateTime.Now.ToString("dd.MM.yyyy hh.mm.ss"));

        string appDir = AppDomain.CurrentDomain.BaseDirectory;
        ExtractionArgs extractionArgs = ExtractionArgs.Parse(Environment.GetCommandLineArgs(), _logBuilder);

        if (extractionArgs == null)
        {
            File.WriteAllText(Path.Combine(appDir, "ZipExtractor.log"), _logBuilder.ToString());
            return;
        }

        appDir = extractionArgs.ExtractionDirectory;
        ExtractionService service = new ExtractionService(_logBuilder, extractionArgs);

        try
        {
            if (extractionArgs.StartExecutableAfterExtraction)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ZipExtractorMainWindow(service));
            }
            else
            {
                _ = await service.Extract().ConfigureAwait(false);
            }
        }
        finally
        {
            File.WriteAllText(Path.Combine(appDir, "ZipExtractor.log"),
                _logBuilder.ToString());
        }

    }
}
