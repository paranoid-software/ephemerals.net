using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ.Management.Client.Model;

namespace paranoid.software.ephemerals.RabbitMQ
{
    public class EphemeralRabbitMqContext: IDisposable
    {

        private readonly IServiceManager _serviceManager;
        
        public string VhostName { get; private set;  }
        public List<Exception> InitializationErrors { get; }
        
        public EphemeralRabbitMqContext(string vhostName, IServiceManager serviceManager, List<Action<EphemeralRabbitMqContext>> actions)
        {
            _serviceManager = serviceManager;
            InitializationErrors = new List<Exception>();
            InitContextWith(vhostName, actions);
        }

        private void InitContextWith(string vhostName, List<Action<EphemeralRabbitMqContext>> actions)
        {
            
            if (vhostName is null)
            {
                VhostName = $"evh_{Guid.NewGuid():N}";
            }
            else
            {
                if (_serviceManager.VhostExists(vhostName))
                {
                    throw new Exception($"Vhost {vhostName} is not available !");
                }
                VhostName = vhostName;
            }
            
            _serviceManager.CreateVhost(VhostName);
            
            foreach(var action in actions)
            {
                try
                {
                    action(this);
                }
                catch (Exception e)
                {
                    InitializationErrors.Add(e);
                }
            }
            
        }
        
        internal void AddExchange(string name, string type, bool autoDelete = false, bool durable = true,
            bool @internal = false, IReadOnlyDictionary<string, object> arguments = null)
        {
            var supportedExchangeTypes = new[] { "direct", "fanout", "headers", "topic" };
            if (!supportedExchangeTypes.Contains(type))
            {
                throw new Exception($"Exchange type {type} is not supported !");
            }
            _serviceManager.AddExchange(name, type, VhostName);
        }

        internal void AddQueue(string name, bool autoDelete = false, bool durable = true,
            IReadOnlyDictionary<string, object> arguments = null)
        {
            _serviceManager.AddQueue(name, VhostName);
        }
        
        internal void AddQueueBinding(string queueName, string exchangeName, string routingKey)
        {
            _serviceManager.AddQueueBinding(queueName, exchangeName, routingKey, VhostName);
        }

        internal void AddDirectQueueMessage(string queueName, byte[] payload, IReadOnlyDictionary<string, object> properties = null)
        {
            _serviceManager.AddQueueMessage(queueName, payload, VhostName);
        }
        
        internal void PublishMessage(string exchangeName, string routingKey, byte[] payload, IReadOnlyDictionary<string, object> properties = null)
        {
            _serviceManager.PublishMessage(exchangeName, routingKey, payload, VhostName);
        }

        public IEnumerable<string> GetAllVhosts()
        {
            return _serviceManager.GetAllVhostsNames();
        }

        public IReadOnlyList<Message> GetMessagesFromQueue(string name, long qty)
        {
            return _serviceManager.GetMessagesFromQueue(name, VhostName, qty);
        }
        
        public void Dispose()
        {
            _serviceManager.DropVhost(VhostName);
        }
        
    }
}