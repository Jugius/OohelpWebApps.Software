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
        public Version Version => new Version(4, 9);
        public string UpdatesServerPath => /*@"https://localhost:7164";*/  @"https://software.oohelp.net";
        public Form MainWindow => this;

        public Uri DownloadPage { get; } = new Uri(@"https://software.oohelp.net");

        private void btnManual_Click(object sender, EventArgs e)
        {
            Task.Run(() => this.deployment.UpdateApplication(UpdateMethod.Manual));
        }
    }
}
