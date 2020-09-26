using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Settings.Settings _settings;

        public SettingsViewModel(INavigationService navigationService, Settings.Settings settings) : base(navigationService)
        {
            Title = "Settings";
            _settings = settings;
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            SettingsDir = _settings.GetSettingsDir();
        }

        private string _settingsDir;

        public string SettingsDir
        {
            get => _settingsDir;
            set { _settingsDir = value; NotifyOfPropertyChange(); }
        }

        public void Save() => NavigateBack();

        public void Cancel() => NavigateBack();
    }
}
