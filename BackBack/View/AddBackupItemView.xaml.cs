using System.Windows;
using BackBack.ViewModel;
using RF.WPF.MVVM;

namespace BackBack.View
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class AddBackupItemView : ViewBase<AddBackupItemViewModel>
    {
        public AddBackupItemView()
        {
            InitializeComponent();
        }
    }
}
