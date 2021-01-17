using System;
using System.Collections.Generic;
using BackBack.Models;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
using RF.WPF.MVVM;
using RF.WPF.Navigation;

namespace BackBack.ViewModel
{
    public class AddAccountViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly AccountStorage _accountStorage;

        public AddAccountViewModel(INavigationService navigationService,
                                   Func<Type, ILogger> loggerFactory,
                                   AccountStorage accountStorage) : base(navigationService)
        {
            Title = "Add New";

            _logger = loggerFactory(typeof(AddAccountViewModel));

            _accountStorage = accountStorage;
        }

        public override void OnNavigatedTo() => base.OnNavigatedTo();

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

        public void Save()
        {
            _logger.LogDebug("Saving");

            if (string.IsNullOrWhiteSpace(Name))
            {
                _logger.LogInformation("Name is null or whitespace");
                return;
            }

            _logger.LogDebug("Creating new {type} with name: '{name}'", typeof(Account), Name);
            var model = new Account();
            PropertySync.Sync(this, model, new HashSet<string>());

            if (_accountStorage.Data.Contains(model))
            {
                _logger.LogInformation("Item with name: '{name}' already exists", Name);
                return;
            }

            _logger.LogDebug("Updating {type} with '{name}'", model.TypeName(), Name);
            _accountStorage.Data.Add(model);
            _accountStorage.Save();

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
