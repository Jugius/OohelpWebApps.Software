
using SoftwareManager.ViewModels.Entities;
using System;
using System.Windows;

namespace SoftwareManager.Dialogs
{
    /// <summary>
    /// Interaction logic for ApplicationPropertiesDialog.xaml
    /// </summary>
    public partial class ApplicationPropertiesDialog : Window
    {
        public ApplicationInfoVM ApplicationInfo { get; private set; }
        public ApplicationPropertiesDialog(ApplicationInfoVM info)
        {
            InitializeComponent();
            this.DataContext = new ApplicationPropertiesDialogVM 
            { Id = info.Id, Name = info.Name, Description = info.Description, IsPublic = info.IsPublic };
        }
        public ApplicationPropertiesDialog()
        {
            InitializeComponent();
            this.DataContext = new ApplicationPropertiesDialogVM
            { Id = Guid.NewGuid() };
        }
        private sealed class ApplicationPropertiesDialogVM : ViewModels.Helpers.ViewModelBase
        {
            public Guid Id { get => id; set { id = value; OnPropertyChanged(nameof(Id)); } }
            private Guid id;
            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
            private string _name;
            public string Description
            {
                get => _description;
                set
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
            private string _description;            

            public bool IsPublic 
            {
                get => isPublic; 
                set
                {
                    isPublic = value; 
                    OnPropertyChanged(nameof(IsPublic));
                }
            }
            private bool isPublic;

            public ApplicationInfoVM GetEntity()
            {
                if(Id == Guid.Empty) throw new Exception("ID не может быть пустым!");
                if (string.IsNullOrEmpty(Name)) throw new Exception("Название не может быть пустым!");
                if (string.IsNullOrEmpty(Description)) throw new Exception("Описание не может быть пустым!");

                return new ApplicationInfoVM
                {
                    Id = this.Id,
                    Name = this.Name,
                    Description = this.Description,
                    IsPublic = this.IsPublic,
                };
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ApplicationInfo = (this.DataContext as ApplicationPropertiesDialogVM)?.GetEntity();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Проверка полей", MessageBoxButton.OK, MessageBoxImage.Error);
            }           
        }
    }
}
