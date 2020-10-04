using System;
using System.Collections.Generic;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Settings _settings;

        public SettingsViewModel(INavigationService navigationService, Settings settings, Func<Type, ILogger> loggerFactory) : base(navigationService)
        {
            _logger = loggerFactory(typeof(SettingsViewModel));

            Title = "Settings";
            _settings = settings;
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            _logger.LogTrace("Opened settings");

            SettingsDir = _settings.GetSettingsDir();

            _logger.LogDebug("Syncing settings properties");
            PropertySync.Sync(_settings.Data, this, new HashSet<string>());
        }

        private string _settingsDir;

        public string SettingsDir
        {
            get => _settingsDir;
            set { _settingsDir = value; NotifyOfPropertyChange(); }
        }

        private bool _closeToTray;
        public bool CloseToTray
        {
            get => _closeToTray;
            set { _closeToTray = value; NotifyOfPropertyChange(); }
        }

        private bool _startWithWindows;
        private ILogger _logger;

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set { _startWithWindows = value; NotifyOfPropertyChange(); }
        }

        public void Save()
        {
            _logger.LogTrace("Saving settings");

            _logger.LogDebug("Syncing settings properties");
            PropertySync.Sync(this, _settings.Data, new HashSet<string>());

            _logger.LogDebug("Saving settings data");
            _settings.Save();

            NavigateBack();
        }

        public void Cancel()
        {
            _logger.LogDebug("Canceling settings");
            NavigateBack();
        }
    }
}
