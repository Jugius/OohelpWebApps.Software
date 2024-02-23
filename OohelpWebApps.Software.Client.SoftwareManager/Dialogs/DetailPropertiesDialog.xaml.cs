using OohelpWebApps.Software.Domain;
using SoftwareManager.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Windows;


namespace SoftwareManager.Dialogs
{
    /// <summary>
    /// Interaction logic for DetailPropertiesDialog.xaml
    /// </summary>
    public partial class DetailPropertiesDialog : Window
    {
        private readonly DialogVM _model;
        public ReleaseDetailVM ReleaseDetail { get; private set; }

        public DetailPropertiesDialog()
        {
            InitializeComponent();
        }
        public DetailPropertiesDialog(ReleaseDetailVM detail) : this()
        {            
            cmbKinds.ItemsSource = new List<DetailKind> 
            {
                DetailKind.Implemented,
                DetailKind.Changed,
                DetailKind.Fixed,
                DetailKind.Updated 
            };
            this.DataContext = _model = new DialogVM(detail);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ReleaseDetail = _model.GetEntity();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Проверка полей", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private sealed class DialogVM : ViewModels.Helpers.ViewModelBase
        {
            public DialogVM(ReleaseDetailVM detail)
            {
                this.Id = detail.Id;
                this.Description = detail.Description;
                this.DetailKind = detail.Kind;
                this.ReleaseId = detail.ReleaseId;
            }
            private DetailKind detailKind;
            private string description;            
            public DetailKind DetailKind { get => detailKind; set { detailKind = value; OnPropertyChanged(nameof(DetailKind)); } }
            public string Description { get => description; set { description = value; OnPropertyChanged(nameof(Description)); } }

            private Guid ReleaseId { get; }
            private Guid Id { get; }

            public ReleaseDetailVM GetEntity()
            {
                if (string.IsNullOrEmpty(this.Description))
                    throw new Exception("Описание не может быть пустым!");
                
                return new ReleaseDetailVM
                {
                    Id = this.Id,
                    Description = this.Description,
                    ReleaseId = this.ReleaseId,
                    Kind = this.DetailKind,
                };
            }
        }
    }
}
