
using System;
using System.Collections.ObjectModel;
using OohelpWebApps.Software.Domain;

namespace SoftwareManager.ViewModels.Entities;

public class ApplicationReleaseVM : Helpers.ViewModelBase
{
    private Version version;
    private DateOnly releaseDate;
    private ReleaseKind kind;

    public Guid Id { get; set; }
    public Version Version { get => version; set { version = value; OnPropertyChanged(nameof(Version)); } }
    public DateOnly ReleaseDate { get => releaseDate; set { releaseDate = value; OnPropertyChanged(nameof(ReleaseDate)); } }
    public ReleaseKind Kind { get => kind; set { kind = value; OnPropertyChanged(nameof(Kind)); } }
    public Guid ApplicationId { get; set; }
    public ObservableCollection<ReleaseDetailVM> Details { get; set; } = new ObservableCollection<ReleaseDetailVM>();
    public ObservableCollection<ReleaseFileVM> Files { get; set; } = new ObservableCollection<ReleaseFileVM>();
}
