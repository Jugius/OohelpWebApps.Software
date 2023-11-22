using System;
using System.Collections.ObjectModel;

namespace SoftwareManager.ViewModels.Entities;

public class ApplicationInfoVM : Helpers.ViewModelBase
{
    private string name;
    private string description;
    private bool isPublic;

    public Guid Id { get; set; }
    public string Name { get => name; set { name = value; OnPropertyChanged(nameof(Name)); } }
    public bool IsPublic { get => isPublic; set { isPublic = value; OnPropertyChanged(nameof(IsPublic)); } }
    public string Description { get => description; set { description = value; OnPropertyChanged(nameof(Description)); } }
    public ObservableCollection<ApplicationReleaseVM> Releases { get; set; } = new ObservableCollection<ApplicationReleaseVM>();
}
