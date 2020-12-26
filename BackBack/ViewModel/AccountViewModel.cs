using System;
using BackBack.Models;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly ILogger _logger;

        public AccountViewModel(INavigationService navigationService,
                                Func<Type, ILogger> loggerFactory) : base(navigationService) => _logger = loggerFactory(typeof(AccountViewModel));

        public Account Account { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; NotifyOfPropertyChange(); }
        }

        private string _login;
        public string Login
        {
            get => _login;
            set { _login = value; NotifyOfPropertyChange(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; NotifyOfPropertyChange(); }
        }

        public override void OnNavigatedTo()
        {
            base.OnNavigatedTo();

            _logger.LogDebug("Syncing Properties with {type}: '{name}'", Account.TypeName(), Account.Name);
            PropertySync.Sync(Account, this, null);
        }
    }
}
