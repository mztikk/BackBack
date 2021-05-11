using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BackBack.Views
{
    public partial class BackupItemsView : UserControl
    {
        public BackupItemsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
