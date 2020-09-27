using System.Diagnostics;
using BackBack.ViewModel;
using Stylet;
using StyletIoC;

namespace BackBack
{
    public class StyletBootstrapper : Bootstrapper<MainViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
            builder.AddModule(new RF.WPF.IocSetup());
            builder.AddModule(new IocSetup());
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
            Container.Get<RF.WPF.IocSetup>().Configure(Container);
        }
    }
}
