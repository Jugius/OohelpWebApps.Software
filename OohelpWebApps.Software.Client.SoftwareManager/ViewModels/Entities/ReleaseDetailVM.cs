
using System;
using OohelpWebApps.Software.Domain;

namespace SoftwareManager.ViewModels.Entities;

public class ReleaseDetailVM : Helpers.ViewModelBase
{
    private DetailKind kind;
    private string description;

    public Guid Id { get; set; }
    public DetailKind Kind { get => kind; set { kind = value; OnPropertyChanged(nameof(Kind)); } }
    public string Description { get => description; set { description = value; OnPropertyChanged(nameof(Description)); } }
    public Guid ReleaseId { get; set; }
}
