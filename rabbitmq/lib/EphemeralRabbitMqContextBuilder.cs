using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace paranoid.software.ephemerals.RabbitMQ
{
    public class EphemeralRabbitMqContextBuilder: IEphemeralRabbitMqContextBuilder
    {

        private readonly IFilesManager _filesManager;
        private readonly List<Action<EphemeralRabbitMqContext>> _actions;

        public EphemeralRabbitMqContextBuilder(IFilesManager filesManager = null)
        {
            _filesManager = filesManager ?? new FilesManager();
            _actions = new List<Action<EphemeralRabbitMqContext>>();
        }
        
        public IEphemeralRabbitMqContextBuilder AddExchange(string name, string type)
        {
            _actions.Add(ctx => ctx.AddExchange(name, type));
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder AddQueue(string name)
        {
            _actions.Add(ctx => ctx.AddQueue(name));
            return this;
        }

        public IEphemeralRabbitMqContextBuilder AddQueueBinding(string queueName, string exchangeName, string routingKey)
        {
            _actions.Add(ctx => ctx.AddQueueBinding(queueName, exchangeName, routingKey));
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder AddDirectQueueMessage(string queueName, byte[] payload)
        {
            _actions.Add(ctx => ctx.AddDirectQueueMessage(queueName, payload));
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder AddDirectQueueMessage(string queueName, string message)
        {
            _actions.Add(ctx => ctx.AddDirectQueueMessage(queueName, Encoding.UTF8.GetBytes(message)));
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder AddDirectQueueMessages(string queueName, List<string> messages)
        {
            foreach (var message in messages)
            {
                _actions.Add(ctx => ctx.AddDirectQueueMessage(queueName, Encoding.UTF8.GetBytes(message)));    
            }
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder AddDirectQueueMessagesFromFile(string queueName, string filepath)
        {
            var jsonArray = _filesManager.ReadJsonArray(filepath);
            foreach (var node in jsonArray)
            {
                _actions.Add(ctx => ctx.AddDirectQueueMessage(queueName, Encoding.UTF8.GetBytes(node.ToJsonString())));
            }
            return this;
        }

        public IEphemeralRabbitMqContextBuilder PublishMessage(string exchangeName, string routingKey, byte[] payload)
        {
            _actions.Add(ctx => ctx.PublishMessage(exchangeName, routingKey, payload));
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder PublishMessage(string exchangeName, string routingKey, string message)
        {
            _actions.Add(ctx => ctx.PublishMessage(exchangeName, routingKey, Encoding.UTF8.GetBytes(message)));
            return this;
        }
        
        public IEphemeralRabbitMqContextBuilder PublishMessages(string exchangeName, string routingKey, List<string> messages)
        {
            foreach (var message in messages)
            {
                _actions.Add(ctx => ctx.PublishMessage(exchangeName, routingKey, Encoding.UTF8.GetBytes(message)));
            }
            return this;
        }

        public IEphemeralRabbitMqContextBuilder PublishMessagesFromFile(string exchangeName, string routingKey, string filepath)
        {
            var jsonArray = _filesManager.ReadJsonArray(filepath);
            foreach (var node in jsonArray)
            {
                _actions.Add(ctx => ctx.PublishMessage(exchangeName, routingKey, Encoding.UTF8.GetBytes(node.ToJsonString())));
            }
            return this;
        }

        public EphemeralRabbitMqContext Build(ConnectionParams connectionParams, string vhost = null,
            IServiceManager serviceManager = null)
        {
            var supportedHostNames = new[] { "localhost", "127.0.0.1" };
            if (!supportedHostNames.Contains(connectionParams.HostName))
                throw new Exception("Ephemeral service must be local, use localhost or 127.0.0.1 as host name.");

            var unsupportedVhosts = new[] { "/" };
            if (unsupportedVhosts.Contains(vhost)) 
                throw new Exception($"Vhost {vhost} is not allowed !");
            
            return new EphemeralRabbitMqContext(vhost, serviceManager ?? new ServiceManager(connectionParams), _actions);
        }
    }
}