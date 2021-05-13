namespace BackBack.Common
{
    public interface IServiceRegistrant
    {
        void Register(LightInject.ServiceContainer container);
    }
}
