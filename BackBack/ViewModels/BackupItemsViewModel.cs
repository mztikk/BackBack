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
            var item = new BackupItemViewModel();
            PropertySync.Sync(dto, item);
            Items.Add(item);
        }
        public ObservableCollection<BackupItemViewModel> Items { get; } = new();
    }
}
