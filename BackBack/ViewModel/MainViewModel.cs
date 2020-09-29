using System;
using System.Collections.Generic;
using BackBack.LUA;
using BackBack.Models;
using BackBack.Storage.Settings;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using RF.WPF.UI.Interaction;
using Stylet;
using StyletIoC;

namespace BackBack.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IContainer _container;
        private readonly BackupData _backupData;
        private readonly Func<Lua> _luaCreator;

        public MainViewModel(IContainer container, INavigationService navigationService, BackupData backupData, Func<Lua> luaCreator) : base(navigationService)
        {
            Title = ApplicationInfo.s_appName;

            _container = container;
            _backupData = backupData;
            _luaCreator = luaCreator;
            BackupItems.Clear();
            foreach (KeyValuePair<string, BackupItem> item in _backupData.Data)
            {
                BackupItems.Add(GetBackupItemViewModel(item.Value));
            }
        }

        private BackupItemViewModel GetBackupItemViewModel(BackupItem backupItem)
        {
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
            EditBackupItemViewModel vm = _container.Get<EditBackupItemViewModel>();
            vm.BackupItem = backupItemViewModel;
            _navigationService.NavigateTo(vm);
        }

        public void AddNewBackupItem()
        {
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
                BackupItems.Remove(backupItem);
                _backupData.Data.Remove(backupItem.Name);
                _backupData.Save();
            }
        }

        public void OpenSettings() => _navigationService.NavigateTo<SettingsViewModel>();
    }
}
