using System.Collections.Generic;

namespace paranoid.software.ephemerals.RabbitMQ
{
    public interface IEphemeralRabbitMqContextBuilder
    {
        IEphemeralRabbitMqContextBuilder AddExchange(string name, string type);
        IEphemeralRabbitMqContextBuilder AddQueue(string name);
        IEphemeralRabbitMqContextBuilder AddQueueBinding(string queueName, string exchangeName, string routingKey);
        IEphemeralRabbitMqContextBuilder AddDirectQueueMessage(string queueName, byte[] payload);
        IEphemeralRabbitMqContextBuilder AddDirectQueueMessage(string queueName, string message);
        IEphemeralRabbitMqContextBuilder AddDirectQueueMessages(string queueName, List<string> messages);
        IEphemeralRabbitMqContextBuilder AddDirectQueueMessagesFromFile(string queueName, string filepath);
        IEphemeralRabbitMqContextBuilder PublishMessage(string exchangeName, string routingKey, byte[] payload);
        IEphemeralRabbitMqContextBuilder PublishMessage(string exchangeName, string routingKey, string message);
        IEphemeralRabbitMqContextBuilder PublishMessages(string exchangeName, string routingKey, List<string> messages);

        IEphemeralRabbitMqContextBuilder PublishMessagesFromFile(string exchangeName, string routingKey,
            string filepath);
        EphemeralRabbitMqContext Build(ConnectionParams connectionParams, string vhost = null, IServiceManager serviceManager = null);
    }
}