using BackBack.Storage.Settings;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class EditBackupItemViewModel : ViewModelBase
    {
        private readonly BackupData _backupData;

        public EditBackupItemViewModel(INavigationService navigationService, BackupData backupData) : base(navigationService)
        {
            Title = "Edit";

            _backupData = backupData;
        }

        public BackupItemViewModel BackupItem { get; set; }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            Name = BackupItem.Name;
            Source = BackupItem.Source;
            Destination = BackupItem.Destination;
            Ignores = BackupItem.Ignores;
            PostCompletionScript = BackupItem.PostCompletionScript;

            Title = $"Edit '{Name}'";
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


        private string _postCompletionScript;
        public string PostCompletionScript
        {
            get => _postCompletionScript;
            set { _postCompletionScript = value; NotifyOfPropertyChange(); }
        }

        public void Save()
        {
            BackupItem.Source = Source;
            BackupItem.Destination = Destination;
            BackupItem.Ignores = Ignores;
            BackupItem.PostCompletionScript = PostCompletionScript;

            BackupItem.BackupItem.Source = Source;
            BackupItem.BackupItem.Destination = Destination;
            BackupItem.BackupItem.Ignores = Ignores;
            BackupItem.BackupItem.PostCompletionScript = PostCompletionScript;

            _backupData.Data[Name] = BackupItem.BackupItem;
            _backupData.Save();

            NavigateBack();
        }

        public void Cancel() => NavigateBack();
    }
}
