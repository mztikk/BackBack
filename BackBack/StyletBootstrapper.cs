using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BackBack.LUA;
using BackBack.Storage;
using BackBack.Storage.Settings;
using BackBack.ViewModel;
using Microsoft.Extensions.Logging;
using RF.WPF;
using RF.WPF.Extensions;
using RLog;
using Stylet;
using StyletIoC;

namespace BackBack
{
    public class StyletBootstrapper : Bootstrapper<MainViewModel>
    {
        private ILogger _logger;

        private static readonly StyletIoCModule[] s_iocModules = new StyletIoCModule[] { new RF.WPF.IocSetup(), new IocSetup(), new StorageIoc(), new LuaIoc() };

        protected override void OnStart()
        {
            using var proc = Process.GetCurrentProcess();
            string location = Path.GetDirectoryName(proc.MainModule.FileName);
            Environment.CurrentDirectory = location;

            base.OnStart();
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            var startup = new StartupInfo
            {
                Args = Args
            };
            startup.StartMinmized = Args.Contains("minimized");
            builder.Bind<StartupInfo>().ToInstance(startup);

            RLogConfigurator config = new RLogConfigurator()
                .SetLoglevel(LogLevel.Debug)
                .AddConsoleOutput()
                .AddStaticFileOutput("logs/BackBack.log")
                .AddFileOutput("logs/context/BackBack.{LogContext}.log");
            LogRProvider provider = new LogRProvider(config);
            Func<Type, ILogger> createLogger = (Type t) => provider.CreateLogger(t.FullName);

            _logger = createLogger(typeof(StyletBootstrapper));

            builder.Bind<Func<Type, ILogger>>().ToInstance(createLogger);

            // Configure the IoC container in here
            foreach (StyletIoCModule item in s_iocModules)
            {
                _logger?.LogDebug("Adding {iocmodule}: '{module}'", nameof(StyletIoCModule), item.TypeName());
                builder.AddModule(item);
            }

            builder.Bind<Tick>().ToSelf().InSingletonScope();
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
            //Container.Get<RF.WPF.IocSetup>().Configure(Container);
            //Container.Get<StorageIoc>().Configure(Container);
            foreach (IocBase item in Container.GetAll<IocBase>())
            {
                _logger?.LogDebug("Configuring {iocmodule}: '{module}'", nameof(IocBase), item.TypeName());
                item.Configure(Container);
            }

            Settings settings = Container.Get<Settings>();
            if (settings.GetValue<bool>("StartWithWindows"))
            {
                Startup.AddToStartup("minimized");
            }

            Container.Get<Tick>();
        }
    }
}
