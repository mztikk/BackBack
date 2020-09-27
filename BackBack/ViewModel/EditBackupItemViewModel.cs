using System.Collections.Generic;
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

            var ignores = new HashSet<string> { "BackupItem" };

            PropertySync.Sync(BackupItem, this, ignores);

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

        private bool _zipFiles;
        public bool ZipFiles
        {
            get => _zipFiles;
            set { _zipFiles = value; NotifyOfPropertyChange(); }
        }

        private string _zipFileDestination;
        public string ZipFileDestination
        {
            get => _zipFileDestination;
            set { _zipFileDestination = value; NotifyOfPropertyChange(); }
        }

        private double _numberOfArchives;
        public double NumberOfArchives
        {
            get => _numberOfArchives;
            set { _numberOfArchives = value; NotifyOfPropertyChange(); }
        }

        private bool _limitArchives;
        public bool LimitArchives
        {
            get => _limitArchives;
            set { _limitArchives = value; NotifyOfPropertyChange(); }
        }

        public void Save()
        {
            var ignores = new HashSet<string> { "BackupItem" };

            PropertySync.Sync(this, BackupItem, ignores);
            PropertySync.Sync(this, BackupItem.BackupItem, ignores);

            _backupData.Data[Name] = BackupItem.BackupItem;
            _backupData.Save();

            NavigateBack();
        }

        public void Cancel() => NavigateBack();
    }
}
