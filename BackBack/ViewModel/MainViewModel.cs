using System.Collections.Generic;
using RF.WPF.MVVM;
using RF.WPF.Navigation;
using Stylet;
using StyletIoC;

namespace BackBack.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IContainer _container;
        private readonly BackupData _backupData;

        public MainViewModel(IContainer container, INavigationService navigationService, BackupData backupData) : base(navigationService)
        {
            _container = container;
            _backupData = backupData;

            BackupItems.Clear();

            foreach (KeyValuePair<string, BackupItem> item in _backupData.Data)
            {
                BackupItems.Add(new BackupItemViewModel(item.Value));
            }
        }

        private BindableCollection<BackupItemViewModel> _backupItems = new BindableCollection<BackupItemViewModel>();

        public BindableCollection<BackupItemViewModel> BackupItems
        {
            get => _backupItems;
            set { _backupItems = value; NotifyOfPropertyChange(); }
        }

        public string Get() => "MainViewModel";

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
            if (vm.BackupItem is { })
            {
                BackupItems.Add(vm.BackupItem);
            }
        }
    }

    public class BackupItem
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Ignores { get; set; }
    }
}
