using System;
using System.Collections.Generic;
using BackBack.Models;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using Stylet;

namespace BackBack.ViewModel
{
    public class EditBackupItemViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly BackupData _backupData;

        public EditBackupItemViewModel(INavigationService navigationService, BackupData backupData, Func<Type, ILogger> loggerFactory) : base(navigationService)
        {
            Title = "Edit";

            _logger = loggerFactory(typeof(EditBackupItemViewModel));

            _backupData = backupData;
        }

        public BackupItemViewModel BackupItem { get; set; }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            Triggers = new BindableCollection<string> { "None", "Cron", "Timed", "BackupItem" };
            var ignores = new HashSet<string> { "BackupItem" };

            _logger.LogDebug("Syncing Properties with {type}: '{name}'", BackupItem.TypeName(), BackupItem.Name);
            PropertySync.Sync(BackupItem, this, ignores);

            BackupNames = new List<string>(_backupData.Data.Keys);

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

        private TriggerInfo _triggerInfo;
        public TriggerInfo TriggerInfo
        {
            get => _triggerInfo;
            set { _triggerInfo = value; NotifyOfPropertyChange(); }
        }

        private BindableCollection<string> _triggers;
        public BindableCollection<string> Triggers
        {
            get => _triggers;
            set { _triggers = value; NotifyOfPropertyChange(); }
        }

        private bool _backupTriggerSettingsVisible;
        public bool BackupTriggerSettingsVisible
        {
            get => _backupTriggerSettingsVisible;
            set { _backupTriggerSettingsVisible = value; NotifyOfPropertyChange(); }
        }

        private bool _timedTriggerSettingsVisible;
        public bool TimedTriggerSettingsVisible
        {
            get => _timedTriggerSettingsVisible;
            set { _timedTriggerSettingsVisible = value; NotifyOfPropertyChange(); }
        }

        private bool _cronTriggerSettingsVisible;
        public bool CronTriggerSettingsVisible
        {
            get => _cronTriggerSettingsVisible;
            set { _cronTriggerSettingsVisible = value; NotifyOfPropertyChange(); }
        }

        private IEnumerable<string> _backupNames;
        public IEnumerable<string> BackupNames
        {
            get => _backupNames;
            set { _backupNames = value; NotifyOfPropertyChange(); }
        }

        public void SelectedTrigger()
        {
            BackupTriggerSettingsVisible = false;
            TimedTriggerSettingsVisible = false;
            CronTriggerSettingsVisible = false;

            switch (TriggerInfo.Type)
            {
                case TriggerType.None:
                    break;
                case TriggerType.TimedTrigger:
                    TimedTriggerSettingsVisible = true;
                    break;
                case TriggerType.BackupItemTrigger:
                    BackupTriggerSettingsVisible = true;
                    break;
                case TriggerType.CronTrigger:
                    CronTriggerSettingsVisible = true;
                    break;
                default:
                    break;
            }
        }

        public void Save()
        {
            _logger.LogDebug("Saving {type}", BackupItem.TypeName());

            var ignores = new HashSet<string> { "BackupItem" };

            _logger.LogDebug("Syncing Properties back to {type}", BackupItem.TypeName());
            PropertySync.Sync(this, BackupItem, ignores);
            _logger.LogDebug("Syncing Properties back to {type}", BackupItem.BackupItem.TypeName());
            PropertySync.Sync(this, BackupItem.BackupItem, ignores);

            _logger.LogDebug("Updating {type} with '{name}'", _backupData.TypeName(), Name);
            _backupData.Data[Name] = BackupItem.BackupItem;
            _backupData.Save();

            _logger.LogTrace("Navigating back");
            NavigateBack();
        }

        public void Cancel()
        {
            _logger.LogDebug("Canceling and navigating back");
            NavigateBack();
        }
    }
}
