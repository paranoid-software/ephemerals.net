using System.Linq;
using FluentAssertions;
using paranoid.software.ephemerals.RabbitMQ;
using Xunit;

namespace tests
{
    public class AddingExchangeShould
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
        public void FailWhenTypeIsNotSupported()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .AddExchange("my-exchange", "unknown")
                .Build(_connectionParams);
            ctx.InitializationErrors.Count.Should().Be(1);
            ctx.InitializationErrors.First().Message.Should().Be($"Exchange type unknown is not supported !");
        }
        
        [Fact]
        public void FailWhenExchangeAlreadyExists()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .AddExchange("one", "topic")
                .AddExchange("one", "direct")
                .Build(_connectionParams);
            ctx.InitializationErrors.Count.Should().Be(1);
        }
        
    }
    
}