using System.Collections.ObjectModel;
using BackBack.Dto;

namespace BackBack.ViewModels
{
    public class BackupItemsViewModel : ViewModelBase
    {
        public BackupItemsViewModel()
        {
            //Items.Add(new() { Name = "REPOS" });
            var dto = new BackupItem() { Name = "REPOS", Source = "E:/repos", Target = "D:/backups/reposbackup" };
            var dto2 = new BackupItem() { Name = "SCRIPTS", Source = "E:/scripts", Target = "D:/backups/scriptsbackup" };
            var item = new BackupItemViewModel();
            var item2 = new BackupItemViewModel();
            PropertySync.Sync(dto, item);
            PropertySync.Sync(dto2, item2);
            Items.Add(item);
            Items.Add(item2);
        }
        public ObservableCollection<BackupItemViewModel> Items { get; } = new();
    }
}
