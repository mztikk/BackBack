using System;
using BackBack.Models;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class AddBackupItemViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly BackupData _backupData;

        public AddBackupItemViewModel(INavigationService navigationService, BackupData backupData, Func<Type, ILogger> loggerFactory) : base(navigationService)
        {
            Title = "Add New";

            _logger = loggerFactory(typeof(AddBackupItemViewModel));

            _backupData = backupData;
        }

        public override void OnNavigatedTo() => base.OnNavigatedTo();

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
            _logger.LogDebug("Saving");

            if (string.IsNullOrWhiteSpace(Name))
            {
                _logger.LogInformation("Name is null or whitespace");
                return;
            }

            if (_backupData.Data.ContainsKey(Name))
            {
                _logger.LogInformation("Item with name: '{name}' already exists", Name);
                return;
            }

            _logger.LogDebug("Creating new {type} with name: '{name}'", typeof(BackupItem), Name);
            var backupItem = new BackupItem { Name = Name, Source = Source, Destination = Destination, Ignores = Ignores };
            _logger.LogDebug("Updating {type} with '{name}'", _backupData.GetType().ToString(), Name);
            _backupData.Data[Name] = backupItem;
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
