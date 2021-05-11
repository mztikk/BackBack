using ReactiveUI.Fody.Helpers;

namespace BackBack.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive]
        public string? TitleText { get; set; } = "BackBack";

        public BackupItemsViewModel BackupItems { get; set; } = new();
    }
}
