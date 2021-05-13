using System.Diagnostics;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BackBack.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(BackupItemsViewModel backupItemsViewModel)
        {
            RunCommand = ReactiveCommand.Create(Run);
            DeleteCommand = ReactiveCommand.Create(Delete);

            BackupItems = backupItemsViewModel;
        }

        [Reactive]
        public string? TitleText { get; set; } = "BackBack";
        [Reactive]
        public ReactiveCommand<Unit, Unit> RunCommand { get; set; }
        [Reactive]
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }

        public BackupItemsViewModel BackupItems { get; set; }

        private void Run()
        {
            foreach (BackupItemViewModel item in BackupItems.GetSelected())
            {
                Debug.WriteLine(item.Name);
            }
        }

        private void Delete()
        {
            foreach (BackupItemViewModel item in BackupItems.GetSelected())
            {
                Debug.WriteLine(item.Name);
            }
        }
    }
}
