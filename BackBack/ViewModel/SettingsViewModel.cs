using System;
using System.Collections.Generic;
using BackBack.Models;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using Stylet;
using StyletIoC;

namespace BackBack.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Settings _settings;
        private readonly AccountStorage _accountStorage;
        private readonly IContainer _container;

        public SettingsViewModel(INavigationService navigationService,
                                 Settings settings,
                                 Func<Type, ILogger> loggerFactory,
                                 AccountStorage accountStorage,
                                 IContainer container) : base(navigationService)
        {
            _logger = loggerFactory(typeof(SettingsViewModel));

            Title = "Settings";
            _settings = settings;
            _accountStorage = accountStorage;
            _container = container;
            Accounts.Clear();
            foreach (Account item in _accountStorage.Data)
            {
                Accounts.Add(GetAccountViewModel(item));
            }
        }

        private BindableCollection<AccountViewModel> _accounts = new BindableCollection<AccountViewModel>();

        public BindableCollection<AccountViewModel> Accounts
        {
            get => _accounts;
            set { _accounts = value; NotifyOfPropertyChange(); }
        }

        private AccountViewModel GetAccountViewModel(Account model)
        {
            _logger.LogDebug("Creating {vm} for {model}: '{modelname}'", nameof(AccountViewModel), nameof(Account), model.Name);
            AccountViewModel vm = _container.Get<AccountViewModel>();
            vm.Account = model;
            vm.OnNavigatedTo();

            return vm;
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

        private bool _minimizeToTray;
        public bool MinimizeToTray
        {
            get => _minimizeToTray;
            set { _minimizeToTray = value; NotifyOfPropertyChange(); }
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
