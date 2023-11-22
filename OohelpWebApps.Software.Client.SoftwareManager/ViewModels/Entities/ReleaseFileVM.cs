

using System;
using OohelpWebApps.Software.Domain;

namespace SoftwareManager.ViewModels.Entities;

public class ReleaseFileVM : Helpers.ViewModelBase
{
    private string name;
    private FileKind kind;
    private FileRuntimeVersion runtimeVersion;
    private string checkSum;
    private int size;
    private DateTime uploaded;
    private string description;

    public Guid Id { get; set; }
    public string Name { get => name; set { name = value; OnPropertyChanged(nameof(Name)); } }
    public FileKind Kind { get => kind; set { kind = value; OnPropertyChanged(nameof(Kind)); } }
    public FileRuntimeVersion RuntimeVersion { get => runtimeVersion; set { runtimeVersion = value; OnPropertyChanged(nameof(RuntimeVersion)); } }
    public string CheckSum { get => checkSum; set { checkSum = value; OnPropertyChanged(nameof(CheckSum)); } }
    public int Size { get => size; set { size = value; OnPropertyChanged(nameof(Size)); } }
    public DateTime Uploaded { get => uploaded; set { uploaded = value; OnPropertyChanged(nameof(Uploaded)); } }
    public string Description { get => description; set { description = value; OnPropertyChanged(nameof(Description)); } }
    public Guid ReleaseId { get; set; }
}
