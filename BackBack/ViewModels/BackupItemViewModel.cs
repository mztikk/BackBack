using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BackBack.ViewModels
{
    public class BackupItemViewModel : ViewModelBase
    {
        [Reactive]
        public string? Name { get; set; }
        [Reactive]
        public string? Source { get; set; }
        [Reactive]
        public string? Target { get; set; }
    }
}
