using OohelpWebApps.Software.Domain;
using SoftwareManager.ViewModels;
using SoftwareManager.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private sealed class DialogVM : ViewModelBase
        {
            public record DetailKindVM(DetailKind Kind, string Name);
            public List<DetailKindVM> DetailKinds { get; }
            public DialogVM(ReleaseDetailVM detail)
            {
                this.DetailKinds = Enum.GetValues<DetailKind>()
                    .Select(a => new DetailKindVM(a, ToValueString(a)))
                    .ToList();

                this.Id = detail.Id;
                this.Description = detail.Description;
                this.DetailKind = new DetailKindVM(detail.Kind, ToValueString(detail.Kind));
                this.ReleaseId = detail.ReleaseId;
            }
            private DetailKindVM detailKind;
            private string description;            
            public DetailKindVM DetailKind { get => detailKind; set { detailKind = value; OnPropertyChanged(nameof(DetailKind)); } }
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
                    Kind = this.DetailKind.Kind,
                };
            }
            private static string ToValueString(DetailKind kind) => kind switch
            {
                OohelpWebApps.Software.Domain.DetailKind.Changed => "Изменения",
                OohelpWebApps.Software.Domain.DetailKind.Fixed => "Исправления",
                OohelpWebApps.Software.Domain.DetailKind.Updated => "Обновления",
                OohelpWebApps.Software.Domain.DetailKind.Implemented => "Новое",
                _ => kind.ToString()
            };
        }
    }
}
