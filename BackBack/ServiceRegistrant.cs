using System;
using BackBack.Common;
using BackBack.ViewModels;
using LightInject;
using RFReborn.RandomR;

namespace BackBack
{
    internal class ServiceRegistrant : IServiceRegistrant
    {
        public void Register(ServiceContainer container) =>
            container
            .Register<Random, CryptoRandom>()
            .Register<ViewLocator>();
    }
}
