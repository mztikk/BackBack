using BackBack.Settings;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class AddBackupItemViewModel : ViewModelBase
    {
        private readonly BackupData _backupData;

        public AddBackupItemViewModel(INavigationService navigationService, BackupData backupData) : base(navigationService)
        {
            Title = "Add New";

            _backupData = backupData;
        }

        public BackupItemViewModel BackupItem { get; set; }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            BackupItem = null;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; NotifyOfPropertyChange(); }
        }


        private string _source;
        public string Source
        {
            get => _source;
            set { _source = value; NotifyOfPropertyChange(); }
        }


        private string _destination;
        public string Destination
        {
            get => _destination;
            set { _destination = value; NotifyOfPropertyChange(); }
        }


        private string _ignores;
        public string Ignores
        {
            get => _ignores;
            set { _ignores = value; NotifyOfPropertyChange(); }
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            if (_backupData.Data.ContainsKey(Name))
            {
                return;
            }

            var backupItem = new BackupItem { Name = Name, Source = Source, Destination = Destination, Ignores = Ignores };
            BackupItem = new BackupItemViewModel(backupItem);
            _backupData.Data[Name] = backupItem;
            _backupData.Save();

            //BackupItem.Source = Source;
            //BackupItem.Destination = Destination;
            //BackupItem.Ignores = Ignores;

            //BackupItem.BackupItem.Source = Source;
            //BackupItem.BackupItem.Destination = Destination;
            //BackupItem.BackupItem.Ignores = Ignores;

            //_backupData.Data[Name] = BackupItem.BackupItem;

            NavigateBack();
        }

        public void Cancel() => NavigateBack();
    }
}
