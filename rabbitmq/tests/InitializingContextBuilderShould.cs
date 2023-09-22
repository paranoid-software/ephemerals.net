using System;
using FluentAssertions;
using paranoid.software.ephemerals.RabbitMQ;
using Xunit;

namespace tests
{
    public class InitializingContextBuilderShould
    {
        [Fact]
        public void ThrowExceptionWhenHostNameIsNotLocal()
        {
            var connectionParams = new ConnectionParams
            {
                HostName = "21.15.24.12",
                ManagementPortNumber = 15672,
                Username = "guest",
                Password = "guest",
                SslEnabled = false
            };
            var exception = Assert.Throws<Exception>(() => new EphemeralRabbitMqContextBuilder().Build(connectionParams));
            exception.Message.Should().Be("Ephemeral service must be local, use localhost or 127.0.0.1 as host name.");        
        }

        [Fact]
        public void ThrowExceptionWhenVhostIsNotAllowed()
        {
            var connectionParams = new ConnectionParams
            {
                HostName = "localhost",
                ManagementPortNumber = 15672,
                Username = "guest",
                Password = "guest",
                SslEnabled = false
            };
            var exception = Assert.Throws<Exception>(() => new EphemeralRabbitMqContextBuilder().Build(connectionParams, "/"));
            exception.Message.Should().Be("Vhost / is not allowed !");
        }
    }
}