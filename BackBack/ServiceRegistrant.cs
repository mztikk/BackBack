using System;
using BackBack.Common;
using LightInject;
using RFReborn.RandomR;

namespace BackBack
{
    public class ServiceRegistrant : IServiceRegistrant
    {
        public void Register(ServiceContainer container) =>
            container
            .Register<Random, CryptoRandom>()
            .Register<ViewLocator>();
    }
}
