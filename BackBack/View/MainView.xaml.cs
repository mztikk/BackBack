using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BackBack.Storage.Settings;
using BackBack.ViewModel;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
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

        private TaskbarIcon _tbi;
        private WindowState _prevState = WindowState.Normal;
        private bool _forceClose = false;
        private ILogger _logger;

        public MainView(Settings settings, StartupInfo startupInfo, Func<Type, ILogger> loggerFactory)
        {
            _logger = loggerFactory(typeof(MainView));
            _settings = settings;
            _startupInfo = startupInfo;

            _logger.LogTrace("Initializing {type}", this.TypeName());

            InitializeComponent();

            var iconUri = new Uri("pack://application:,,,/BackBack;component/Resources/app_icon.ico");
            _logger.LogDebug("Loading icon from '{uri}'", iconUri.ToString());
            Stream iconStream = Application.GetResourceStream(iconUri).Stream;
            var icon = new Icon(iconStream);
            iconStream.Dispose();

            _logger.LogDebug("Creating tray icon");
            _tbi = new TaskbarIcon
            {
                Icon = icon,
                Visibility = Visibility.Visible,
            };
            _tbi.TrayMouseDoubleClick += Tbi_TrayMouseDoubleClick;
            var ctxMenu = new ContextMenu();
            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += ExitItem_Click;
            ctxMenu.Items.Add(exitItem);
            _tbi.ContextMenu = ctxMenu;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            _tbi.ToolTipText = Title;

            _logger.LogInformation("StartMinimized is '{value}'", _startupInfo.StartMinmized);
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

        private void ToTray()
        {
            _logger.LogDebug("Minimizing window to tray");
            WindowState = WindowState.Minimized;
            Hide();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            bool MinimizeToTray = _settings.GetValue<bool>("MinimizeToTray");
            _logger.LogDebug("MinimizeToTray is '{value}'", MinimizeToTray);

            if (WindowState == WindowState.Minimized && MinimizeToTray)
            {
                ToTray();
            }

            base.OnStateChanged(e);
        }
    }
}
