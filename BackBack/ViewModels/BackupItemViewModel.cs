using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BackBack.ViewModels
{
    public class BackupItemViewModel : ViewModelBase
    {
        public BackupItemViewModel() => PressedCommand = ReactiveCommand.Create<PointerPressedEventArgs>(Pressed);

        [Reactive]
        public string? Name { get; set; }
        [Reactive]
        public string? Source { get; set; }
        [Reactive]
        public string? Target { get; set; }
        [Reactive]
        public bool Selected { get; set; }
        [Reactive]
        public bool Hovered { get; set; }
        [Reactive]
        public ReactiveCommand<PointerPressedEventArgs, System.Reactive.Unit> PressedCommand { get; set; }

        private void Pressed(PointerPressedEventArgs e) => Selected = !Selected;
    }
}
