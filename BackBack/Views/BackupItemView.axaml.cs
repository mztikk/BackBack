using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BackBack.ViewModels;

namespace BackBack.Views
{
    public partial class BackupItemView : UserControl
    {
        public BackupItemView() => InitializeComponent();

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        public void PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (DataContext is BackupItemViewModel vm)
            {
                vm.Selected = !vm.Selected;
            }
        }

        public void PointerEnter(object sender, PointerEventArgs e)
        {
            if (DataContext is BackupItemViewModel vm)
            {
                vm.Hovered = true;
            }
        }
        public void PointerLeave(object sender, PointerEventArgs e)
        {
            if (DataContext is BackupItemViewModel vm)
            {
                vm.Hovered = false;
            }
        }
    }
}
