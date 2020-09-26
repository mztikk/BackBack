using System.Windows;
using BackBack.ViewModel;
using RF.WPF.MVVM;

namespace BackBack.View
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class SettingsView : ViewBase<SettingsViewModel>
    {
        public SettingsView(SettingsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
