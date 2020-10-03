using System.Collections.Generic;
using BackBack.Storage.Settings;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Settings _settings;

        public SettingsViewModel(INavigationService navigationService, Settings settings) : base(navigationService)
        {
            Title = "Settings";
            _settings = settings;
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            SettingsDir = _settings.GetSettingsDir();

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
        public bool StartWithWindows
        {
            get => _startWithWindows;
            set { _startWithWindows = value; NotifyOfPropertyChange(); }
        }

        public void Save()
        {
            PropertySync.Sync(this, _settings.Data, new HashSet<string>());
            _settings.Save();

            NavigateBack();
        }

        public void Cancel() => NavigateBack();
    }
}
