using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

namespace OohelpWebApps.Software.Updater.WinForms.Dialogs;
internal partial class UpdateInfoDialog : Form
{
    public UpdateInfoDialog()
    {
        InitializeComponent();
        pictureBox1.Image = LoadFrom("../../Icons/icon_attention_24px.png");
        pictureBox2.Image = LoadFrom("../../Icons/icon_downloading_updates_48px.png");
    }

    private static Image LoadFrom(string path)
    {
        Image image = new Bitmap(path);
        return image;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ApplicationName 
    {
        get => lblAppName.Text; 
        set => lblAppName.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UpdateVersion
    { 
        get => lblUpdateVersion.Text; 
        set => lblUpdateVersion.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UpdateSize
    {
        get => lblUpdateSize.Text;
        set => lblUpdateSize.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UpdateStatus
    { 
        get => lblUpdateStatus.Text; 
        set => lblUpdateStatus.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Uri UpdateDetailsUri { get; internal set; }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string CurrentVersion
    { 
        get => lblCurrentVersion.Text;
        set =>lblCurrentVersion.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string LastTimeUpdated
    {
        get => lblLastTimeUpdated.Text; 
        set => lblLastTimeUpdated.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UpdateDescription
    { 
        get=>lblUpdateDescription.Text;
        set => lblUpdateDescription.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string AttentionMessage
    { 
        get => lblAttentionText.Text;
        set => lblAttentionText.Text = value;
    }


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
