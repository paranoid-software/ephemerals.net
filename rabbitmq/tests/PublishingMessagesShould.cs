using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using paranoid.software.ephemerals.RabbitMQ;
using Xunit;

namespace tests
{
    public class PublishingMessagesShould
    {
        
        private readonly ConnectionParams _connectionParams = new ConnectionParams
        {
            HostName = "localhost",
            ManagementPortNumber = 15672,
            Username = "guest",
            Password = "guest",
            SslEnabled = false
        };
        
        [Fact]
        public void FailWhenExchangeDoesNotExist()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .PublishMessage("books", "", "This is a new message !")
                .Build(_connectionParams);
            ctx.InitializationErrors.Count.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public void Post0MessagesToQueueWhenRoutingKeyDoesNotMatch()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .AddExchange("todo", "topic")
                .AddQueue("tasks")
                .AddQueueBinding("tasks", "todo", "tasks.with.priority.5")
                .PublishMessage("todo", "tasks.with.priority.3", "Code new catering system")
                .Build(_connectionParams);
            ctx.GetMessagesFromQueue("tasks", 10).Count.Should().Be(0);
        }
        
        [Fact]
        public void Post2MessagesToQueue()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .AddExchange("todo", "topic")
                .AddQueue("tasks")
                .AddQueueBinding("tasks", "todo", "tasks.with.priority.5")
                .PublishMessages("todo", "tasks.with.priority.5", new List<string> { "Mi first task" })
                .PublishMessage("todo", "tasks.with.priority.5", Encoding.UTF8.GetBytes("Another cool task"))
                .Build(_connectionParams);
            ctx.GetMessagesFromQueue("tasks", 10).Count.Should().Be(2);
        }
        
    }
    
}