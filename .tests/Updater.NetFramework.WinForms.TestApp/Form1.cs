using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using OohelpWebApps.Software.Updater;

namespace Updater.NetFramework.WinForms.UITests
{
    public partial class Form1 : Form, IUpdatableApplication
    {
        private readonly ApplicationDeployment deployment;
        public Form1()
        {
            InitializeComponent();
            this.deployment = new ApplicationDeployment(this);
        }

        public string ApplicationName => "AdvisorDB";
        public Version Version => new Version(4, 11);
        public Uri UpdatesServer { get; } = new Uri("https://localhost:7164"); // new Uri("https://software.oohelp.net");
        public Form MainWindow => this;

        public Uri DownloadPage { get; } = new Uri(@"https://oohelp.net/software/advisordb/download");

        private void btnManual_Click(object sender, EventArgs e)
        {
            Task.Run(() => this.deployment.UpdateApplication(UpdateMethod.Manual));
        }

        private void btnAutomatic_Click(object sender, EventArgs e)
        {
            Task.Run(() => this.deployment.UpdateApplication(UpdateMethod.Automatic));
        }

        private void btnAutomaticDownload_UpdateOnRequest_Click(object sender, EventArgs e)
        {
            Task.Run(() => this.deployment.UpdateApplication(UpdateMethod.AutomaticDownload_UpdateOnRequest));
        }

        private void btnDownloadAndUpdateOnRequest_Click(object sender, EventArgs e)
        {
            Task.Run(() => this.deployment.UpdateApplication(UpdateMethod.DownloadAndUpdateOnRequest));
        }

        private void btnGetStatus_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.deployment.ToString(), "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
