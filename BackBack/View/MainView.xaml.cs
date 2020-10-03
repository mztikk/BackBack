using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BackBack.Storage.Settings;
using BackBack.ViewModel;
using Hardcodet.Wpf.TaskbarNotification;
using RF.WPF.MVVM;

namespace BackBack.View
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class MainView : ViewBase<MainViewModel>
    {
        private readonly Settings _settings;
        private readonly StartupInfo _startupInfo;
        private WindowState _prevState = WindowState.Normal;
        private bool _forceClose = false;

        public MainView(Settings settings, StartupInfo startupInfo)
        {
            InitializeComponent();

            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/BackBack;component/Resources/app_icon.ico")).Stream;
            var icon = new Icon(iconStream);
            iconStream.Dispose();

            TaskbarIcon tbi = new TaskbarIcon
            {
                Icon = icon,
                Visibility = Visibility.Visible,
            };
            tbi.TrayMouseDoubleClick += Tbi_TrayMouseDoubleClick;
            var ctxMenu = new ContextMenu();
            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += ExitItem_Click;
            ctxMenu.Items.Add(exitItem);
            tbi.ContextMenu = ctxMenu;
            _settings = settings;
            _startupInfo = startupInfo;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_startupInfo.StartMinmized)
            {
                ToTray();
            }
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            _forceClose = true;
            Close();
        }

        private void Tbi_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = _prevState;
            Activate();
        }

        private void ViewBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_forceClose && _settings.GetValue<bool>("CloseToTray"))
            {
                _prevState = WindowState;
                e.Cancel = true;
                ToTray();
            }
        }

        private void ToTray()
        {
            WindowState = WindowState.Minimized;
            Hide();
        }
    }
}
