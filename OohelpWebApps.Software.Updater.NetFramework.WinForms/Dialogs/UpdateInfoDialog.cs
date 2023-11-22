using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OohelpWebApps.Software.Updater.Dialogs;
internal partial class UpdateInfoDialog : Form
{
    public UpdateInfoDialog()
    {
        InitializeComponent();
    }

    public string ApplicationName 
    {
        get => lblAppName.Text; 
        set => lblAppName.Text = value;
    }
    public string UpdateVersion
    { 
        get => lblUpdateVersion.Text; 
        set => lblUpdateVersion.Text = value;
    }
    public string UpdateSize
    {
        get => lblUpdateSize.Text;
        set => lblUpdateSize.Text = value;
    }
    public string UpdateStatus
    { 
        get => lblUpdateStatus.Text; 
        set => lblUpdateStatus.Text = value;
    }
    public Uri UpdateDetailsUri { get; internal set; }
    public string CurrentVersion
    { 
        get => lblCurrentVersion.Text;
        set =>lblCurrentVersion.Text = value;
    }
    public string LastTimeUpdated
    {
        get => lblLastTimeUpdated.Text; 
        set => lblLastTimeUpdated.Text = value;
    }
    public string UpdateDescription
    { 
        get=>lblUpdateDescription.Text;
        set => lblUpdateDescription.Text = value;
    }
    public string AttentionMessage
    { 
        get => lblAttentionText.Text;
        set => lblAttentionText.Text = value;
    }
    public string NewFeatures
    { 
        get => lblNewFeatures.Text;
        set => lblNewFeatures.Text = value;
    }


    private void lnkUpdateDetailsUri_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (this.UpdateDetailsUri != null)
        { 
            ProcessStartInfo process = new ProcessStartInfo 
            {
                FileName = this.UpdateDetailsUri.AbsoluteUri,
                UseShellExecute = true,
            };
            Process.Start(process);
        }
    }
}
