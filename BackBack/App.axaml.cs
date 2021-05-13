using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BackBack.ViewModels;
using BackBack.Views;
using LightInject;

namespace BackBack
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            var container = new ServiceContainer();
            container.RegisterInstance(container);

            ServiceRegistree.Register(container);

            DataTemplates.Add(container.GetInstance<ViewLocator>());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = container.GetInstance<MainWindowViewModel>(),
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
