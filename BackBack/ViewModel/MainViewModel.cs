using System;
using System.Collections.Generic;
using BackBack.Models;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using RF.WPF.UI.Interaction;
using Stylet;
using StyletIoC;

namespace BackBack.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly IContainer _container;
        private readonly BackupData _backupData;
        private readonly IWindowManager _windowManager;

        public MainViewModel(IContainer container, INavigationService navigationService, BackupData backupData, Func<Type, ILogger> loggerFactory, IWindowManager windowManager) : base(navigationService)
        {
            Title = ApplicationInfo.s_appName;

            _logger = loggerFactory(typeof(MainViewModel));

            _container = container;
            _backupData = backupData;
            _windowManager = windowManager;
            BackupItems.Clear();
            foreach (KeyValuePair<string, BackupItem> item in _backupData.Data)
            {
                BackupItems.Add(GetBackupItemViewModel(item.Value));
            }
        }

        private BackupItemViewModel GetBackupItemViewModel(BackupItem backupItem)
        {
            _logger.LogDebug("Creating {backupvm} for {backupitem}: '{backupitem}'", nameof(BackupItemViewModel), nameof(BackupItem), backupItem.Name);
            BackupItemViewModel backupItemVM = _container.Get<BackupItemViewModel>();
            backupItemVM.BackupItem = backupItem;
            backupItemVM.OnNavigatedTo();

            return backupItemVM;
        }

        private BindableCollection<BackupItemViewModel> _backupItems = new BindableCollection<BackupItemViewModel>();

        public BindableCollection<BackupItemViewModel> BackupItems
        {
            get => _backupItems;
            set { _backupItems = value; NotifyOfPropertyChange(); }
        }

        public void EditBackupItem(BackupItemViewModel backupItemViewModel)
        {
            _logger.LogDebug("Editing '{type}': '{name}'", backupItemViewModel.TypeName(), backupItemViewModel.Name);
            EditBackupItemViewModel vm = _container.Get<EditBackupItemViewModel>();
            vm.BackupItem = backupItemViewModel;
            _navigationService.NavigateTo(vm);
        }

        public void AddNewBackupItem()
        {
            _logger.LogDebug("Adding new '{type}", typeof(BackupItemViewModel).ToString());
            AddBackupItemViewModel vm = _container.Get<AddBackupItemViewModel>();
            _navigationService.NavigateTo(vm);
            if (vm.Name is { } && _backupData.Data.ContainsKey(vm.Name))
            {
                BackupItems.Add(GetBackupItemViewModel(_backupData.Data[vm.Name]));
            }
        }

        public void RemoveItem(BackupItemViewModel backupItem)
        {
            if (_navigationService.GetConfirmation("DELETE", $"Are you sure you want to delete '{backupItem.Name}'?", ConfirmationButtonInfo.NoDelete) == ConfirmationResult.Affirmative)
            {
                backupItem.Dispose();
                foreach (BackupItemViewModel item in BackupItems)
                {
                    TriggerInfo trigger = item.BackupItem.TriggerInfo;
                    if (trigger.Type == TriggerType.BackupItemTrigger && trigger.BackupName == backupItem.Name)
                    {
                        trigger.Type = TriggerType.None;
                        trigger.BackupName = null;
                    }
                }
                BackupItems.Remove(backupItem);
                _backupData.Data.Remove(backupItem.Name);
                _backupData.Save();
            }
        }

        public void OpenSettings() => _navigationService.NavigateTo<SettingsViewModel>();
    }
}
