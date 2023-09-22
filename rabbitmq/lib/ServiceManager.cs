using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;


namespace paranoid.software.ephemerals.RabbitMQ
{
    internal class ServiceManager: IServiceManager
    {
        
        private readonly ConnectionParams _connectionParams;
        
        public ServiceManager(ConnectionParams connectionParams)
        {
            _connectionParams = connectionParams;
        }

        public bool VhostExists(string name)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            try
            {
                client.GetVhost(name);
            }
            catch (UnexpectedHttpStatusCodeException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
            }
            return true;
        }
        
        public void CreateVhost(string name)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            client.CreateVhost(name);
        }
        
        public void AddExchange(string name, string type, string vhostName, bool autoDelete = false, bool durable = true, bool @internal = false, IReadOnlyDictionary<string, object> arguments = null)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            var vhost = client.GetVhost(vhostName);
            client.CreateExchange(new ExchangeInfo(name, type, autoDelete, durable, @internal, arguments), vhost);
        }

        public void AddQueue(string name, string vhostName, bool autoDelete = false, bool durable = true, IReadOnlyDictionary<string, object> arguments = null)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            var vhost = client.GetVhost(vhostName);
            client.CreateQueue(new QueueInfo(name, autoDelete, durable, arguments), vhost);
        }

        public void AddQueueBinding(string queueName, string exchangeName, string routingKey, string vhostName)
        {   
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            var vhost = client.GetVhost(vhostName);
            var queue = client.GetQueue(vhost, queueName);
            var exchange = client.GetExchange(exchangeName, vhost);
            client.CreateQueueBinding(exchange, queue, new BindingInfo(routingKey));
        }

        public void AddQueueMessage(string queueName, byte[] payload, string vhostName, IReadOnlyDictionary<string, object> properties = null)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            var result = client.Publish(new Exchange("", vhostName, "direct"), new PublishInfo(queueName, Encoding.UTF8.GetString(payload), Properties: properties));
            if (!result.Routed) throw new Exception("Message could not be routed !");
        }
        
        public void PublishMessage(string exchangeName, string routingKey, byte[] payload, string vhostName, IReadOnlyDictionary<string, object> properties = null)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            var vhost = client.GetVhost(vhostName);
            var exchange = client.GetExchange(exchangeName, vhost);
            var result = client.Publish(exchange, new PublishInfo(routingKey, Encoding.UTF8.GetString(payload), Properties: properties));
            if (!result.Routed) throw new Exception("Message could not be routed !");
        }
        
        public IEnumerable<string> GetAllVhostsNames()
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            return client.GetVhosts().Select(x=>x.Name).ToList();
        }

        public IReadOnlyList<Message> GetMessagesFromQueue(string name, string vhostName, long qty, AckMode ackMode = AckMode.AckRequeueTrue)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            var vhost = client.GetVhost(vhostName);
            var queue = client.GetQueue(vhost, name);
            return client.GetMessagesFromQueue(queue, new GetMessagesFromQueueInfo(qty, ackMode));
        }

        public void DropVhost(string name)
        {
            using var client = new ManagementClient(_connectionParams.GetManagementBaseUri(), _connectionParams.Username, _connectionParams.Password);
            client.DeleteVhost(new Vhost(name));
        }
        
    }
    
}
