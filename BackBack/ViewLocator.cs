using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BackBack.ViewModels;
using LightInject;

namespace BackBack
{
    public class ViewLocator : IDataTemplate
    {
        private readonly ServiceContainer _container;

        public ViewLocator(ServiceContainer container)
        {
            _container = container;
        }

        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                //return (Control)Activator.CreateInstance(type)!;
                return (IControl)_container.GetInstance(type);
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
