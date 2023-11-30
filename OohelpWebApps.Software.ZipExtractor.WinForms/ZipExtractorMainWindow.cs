namespace OohelpWebApps.Software.ZipExtractor.NetCore.WinForms;

public partial class ZipExtractorMainWindow : Form
{
    private readonly CancellationTokenSource tokenSource;
    private readonly ExtractionService extractionService;
    public ZipExtractorMainWindow()
    {
        InitializeComponent();
    }
    public ZipExtractorMainWindow(ExtractionService extractionService) : this()
    {
        this.extractionService = extractionService;
        this.tokenSource = new CancellationTokenSource();
        this.Text = extractionService.ApplicationName + " Installer";
    }
    private async void ZipExtractorDialog_ShownAsync(object sender, EventArgs e)
    {
        Progress<ExtractionProgress> progress = new Progress<ExtractionProgress>(UpdateProgress);

        try
        {
            var result = await extractionService.Extract(tokenSource.Token, progress);

            if (result.IsSuccess)
            {
                UpdateProgress(new ExtractionProgress("Завершено"));
                extractionService.StartExecutable();
                return;
            }

            if (result.Cancelled)
            {
                MessageBox.Show($"Распаковка отменена", "Отмена установки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (result.Error != null)
            {
                MessageBox.Show($"{result.Error.GetType()}: {result.Error.Message}", "Ошибка установки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        finally
        {
            Application.Exit();
        }
    }

    private void UpdateProgress(ExtractionProgress progress)
    {
        if (progress.Progress == -1)
        {
            if (this.progressBar.Style != ProgressBarStyle.Marquee)
                this.progressBar.Style = ProgressBarStyle.Marquee;

            this.lblCurrentOperation.Text = progress.Operation;
            this.lblCurrentProgress.Text = null;
        }
        else
        {
            if (this.progressBar.Style != ProgressBarStyle.Continuous)
                this.progressBar.Style = ProgressBarStyle.Continuous;

            this.progressBar.Value = progress.Progress;
            this.lblCurrentProgress.Text = progress.Operation;
        }

    }
    private void CancelButton_Click(object sender, EventArgs e)
    {
        this.tokenSource.Cancel();
    }
}
