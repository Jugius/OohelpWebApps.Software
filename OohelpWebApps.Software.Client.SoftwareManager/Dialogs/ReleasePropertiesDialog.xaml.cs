using OohelpWebApps.Software.Domain;
using SoftwareManager.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SoftwareManager.Dialogs
{
    /// <summary>
    /// Interaction logic for ReleasePropertiesDialog.xaml
    /// </summary>
    public partial class ReleasePropertiesDialog : Window
    {
        private ApplicationReleaseVM _release;
        private readonly ReleasePropertiesDialogVM _model;
        public ApplicationReleaseVM Release => _release;
        public ReleasePropertiesDialog()
        {
            InitializeComponent();
            cmbKinds.ItemsSource = new List<ReleaseKind>
            {
                ReleaseKind.Minor,
                ReleaseKind.Major,
                ReleaseKind.Patch,
                ReleaseKind.Critical
            };
        }
        public ReleasePropertiesDialog(ApplicationReleaseVM release) : this()
        {
            _release = release;   

            _model = new ReleasePropertiesDialogVM
            {
                ReleaseKind = release.Kind,
                ReleaseDate = release.ReleaseDate.ToDateTime(TimeOnly.MinValue)
            };
            Version v = release.Version;
            _model.Major = v.Major;
            _model.Minor = v.Minor;
            _model.Build = v.Build;
            _model.Revision = v.Revision;

            this.DataContext = _model;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            ApplicationReleaseVM newRelease = new ApplicationReleaseVM
            {
                Id = _release.Id,
                ApplicationId = _release.ApplicationId,
                Kind = _model.ReleaseKind,
                ReleaseDate = DateOnly.FromDateTime(_model.ReleaseDate),
                Version = _model.GetVersion(),
            };

            _release = newRelease;
            this.DialogResult = true;
        }
        private sealed class ReleasePropertiesDialogVM : ViewModels.Helpers.ViewModelBase
        {
            private int major;
            private int minor;
            private int build;
            private int revision;
            private DateTime releaseDate;
            private ReleaseKind releaseKind;

            public int Major { get => major; set { major = value; OnPropertyChanged(nameof(Major)); } }
            public int Minor { get => minor; set { minor = value; OnPropertyChanged(nameof(Minor)); } }
            public int Build { get => build; set { build = value; OnPropertyChanged(nameof(Build)); } }
            public int Revision { get => revision; set { revision = value; OnPropertyChanged(nameof(Revision)); } }
            public DateTime ReleaseDate { get => releaseDate; set { releaseDate = value; OnPropertyChanged(nameof(ReleaseDate)); } }
            public ReleaseKind ReleaseKind { get => releaseKind; set { releaseKind = value; OnPropertyChanged(nameof(ReleaseKind)); } }

            public Version GetVersion() => GetVersion(4);
            public Version GetVersion(byte position)
            {
                return position switch
                {
                    1 => Major > 0 ? new Version(Major, 0) : throw new ArgumentException("Major"),
                    2 => Minor >= 0 ? new Version(Major, Minor) : GetVersion(1),
                    3 => Build >= 0 ? new Version(Major, Minor, Build) : GetVersion(2),
                    4 => Revision >= 0 ? new Version(Major, Minor, Build, Revision) : GetVersion(3),
                    _ => throw new Exception(">4")
                };
            }
        }
    }
}
