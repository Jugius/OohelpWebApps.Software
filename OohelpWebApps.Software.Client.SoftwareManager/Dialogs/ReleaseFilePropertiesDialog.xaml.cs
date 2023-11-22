using OohelpWebApps.Software.Domain;
using SoftwareManager.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SoftwareManager.Dialogs;

/// <summary>
/// Interaction logic for ReleaseFilePropertiesDialog.xaml
/// </summary>
public partial class ReleaseFilePropertiesDialog : Window
{
    public ReleaseFilePropertiesDialog()
    {
        InitializeComponent();

        cmbKinds.ItemsSource = new List<FileKind>
        {
            FileKind.Install,
            FileKind.Update
        };

        cmbRuntimeVersions.ItemsSource = new List<FileRuntimeVersion>
        {
            FileRuntimeVersion.NetFramework,
            FileRuntimeVersion.Net5,
            FileRuntimeVersion.Net6
        };
    }
    public ReleaseFilePropertiesDialog(ReleaseFileVM file) : this()
    {
        if (this.DataContext is ReleaseFileProperties context)
        {
            context.Name = file.Name;
            context.Description = file.Description;           
            context.Kind = file.Kind;
            context.RuntimeVersion = file.RuntimeVersion;
        }
    }
    public ReleaseFileVM ReleaseFile { get; private set; }

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.ReleaseFile = (this.DataContext as ReleaseFileProperties)?.GetEntity();
            this.DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Проверка полей", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
public class ReleaseFileProperties : ViewModels.Helpers.ViewModelBase
{
    private FileKind kind;
    private string description;
    private FileRuntimeVersion runtimeVersion;

    public string Name { get; set; }
    public FileKind Kind { get => kind; set { kind = value; OnPropertyChanged(nameof(Kind)); } }
    public FileRuntimeVersion RuntimeVersion { get => runtimeVersion; set { runtimeVersion = value; OnPropertyChanged(nameof(RuntimeVersion)); } }
    public string Description { get => description; set { description = value; OnPropertyChanged(nameof(Description)); } }
    public ReleaseFileVM GetEntity()
    {
        if (string.IsNullOrEmpty(this.Description))
            throw new Exception("Описание не может быть пустым!");

        return new ReleaseFileVM
        {
            Name = this.Name,
            Kind = this.Kind,
            RuntimeVersion = this.RuntimeVersion,
            Description = this.Description
        };
    }
}
