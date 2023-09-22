using System.Collections.Generic;
using EasyNetQ.Management.Client.Model;

namespace paranoid.software.ephemerals.RabbitMQ
{
    public interface IServiceManager
    {
        bool VhostExists(string name);
        void CreateVhost(string name);

        void AddExchange(string name, string type, string vhostName, bool autoDelete = false, bool durable = true,
            bool @internal = false, IReadOnlyDictionary<string, object> arguments = null);

        void AddQueue(string name, string vhostName, bool autoDelete = false, bool durable = true,
            IReadOnlyDictionary<string, object> arguments = null);
        void AddQueueBinding(string queueName, string exchangeName, string routingKey, string vhostName);
        void AddQueueMessage(string queueName, byte[] payload, string vhostName, IReadOnlyDictionary<string, object> properties = null);
        void PublishMessage(string exchangeName, string routingKey, byte[] payload, string vhostName, IReadOnlyDictionary<string, object> properties = null);
        IEnumerable<string> GetAllVhostsNames();

        IReadOnlyList<Message> GetMessagesFromQueue(string name, string vhostName, long qty,
            AckMode ackMode = AckMode.AckRequeueTrue);
        void DropVhost(string name);
    }
}