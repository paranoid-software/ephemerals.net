namespace paranoid.software.ephemerals.RabbitMQ
{
    public interface IServiceManager
    {
        void CreateVirtualHost(string name);
        void DropVirtualHost(string name);
    }
}