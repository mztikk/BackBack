using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BackBack.ViewModels
{
    public class BackupItemViewModel : ViewModelBase
    {
        public BackupItemViewModel()
        {
            PressedCommand = ReactiveCommand.Create(Pressed);
        }

        [Reactive]
        public string? Name { get; set; }
        [Reactive]
        public string? Source { get; set; }
        [Reactive]
        public string? Target { get; set; }
        [Reactive]
        public bool Selected { get; set; }
        [Reactive]
        public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> PressedCommand { get; set; }

        private void Pressed() => System.Console.WriteLine("pressed");
    }
}
