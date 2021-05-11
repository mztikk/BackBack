using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BackBack.Views
{
    public partial class BackupItemView : UserControl
    {
        public BackupItemView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
