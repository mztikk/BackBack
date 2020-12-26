using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Threading;
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
        private Func<Type, ILogger> _createLogger;

        private static readonly StyletIoCModule[] s_iocModules = new StyletIoCModule[] { new RF.WPF.IocSetup(), new IocSetup(), new StorageIoc(), new LuaIoc() };

        protected override void OnStart()
        {
            using var proc = Process.GetCurrentProcess();
            string location = Path.GetDirectoryName(proc.MainModule.FileName);
            Environment.CurrentDirectory = location;

            LogLevel logLevel;
#if DEBUG
            logLevel = LogLevel.Debug;
#else
            logLevel = LogLevel.Information;
#endif

            RLogConfigurator config = new RLogConfigurator()
                .SetLoglevel(logLevel)
                .AddConsoleOutput()
                .AddStaticFileOutput("logs/BackBack.log")
                .AddFileOutput("logs/context/BackBack.{LogContext}.log");
            LogRProvider provider = new LogRProvider(config);
            _createLogger = (Type t) => provider.CreateLogger(t.FullName);

            _logger = _createLogger(typeof(StyletBootstrapper));
            _logger.LogInformation("Logger initialized with LogLevel {loglevel}", logLevel);

            base.OnStart();
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            _logger.LogInformation("Configuring IoC");

            _logger.LogInformation("Setting {type}", typeof(StartupInfo).ToString());
            var startup = new StartupInfo
            {
                Args = Args
            };
            startup.StartMinmized = Args.Contains("minimized");
            builder.Bind<StartupInfo>().ToInstance(startup);

            builder.Bind<Func<Type, ILogger>>().ToInstance(_createLogger);

            builder.Bind<IEntropy>().ToInstance(new AssemblyNameEntropy());

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
            bool startWithWindows = settings.GetValue<bool>("StartWithWindows");
            _logger.LogInformation("StartWithWindows is '{value}'", startWithWindows);
            if (settings.GetValue<bool>("StartWithWindows"))
            {
                _logger.LogInformation("Adding current instance to startup");
                Startup.AddToStartup("minimized");
            }

            Container.Get<Tick>();
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogCritical(e.Exception, "Unhandled Exception: {ex}", e.Exception.GetBaseException().ToString());

            base.OnUnhandledException(e);
        }
    }
}
