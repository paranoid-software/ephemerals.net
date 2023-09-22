using System;
using FluentAssertions;
using Moq;
using paranoid.software.ephemerals.RabbitMQ;
using Xunit;

namespace tests
{
    public class BuildingContextShould
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
        public void ThrowExceptionWhenVhostIsNotAvailable()
        {
            using var ctx1 = new EphemeralRabbitMqContextBuilder().Build(_connectionParams, "my_cool_vhost");
            var exception = Assert.Throws<Exception>(() =>
            {
                using var ctx2 = new EphemeralRabbitMqContextBuilder().Build(_connectionParams, "my_cool_vhost");
            });
            exception.Message.Should().Be("Vhost my_cool_vhost is not available !");
        }
        
        [Fact]
        public void SetEphemeralVhostPrefix()
        {
            var serviceManagerMock = new Mock<IServiceManager>(MockBehavior.Strict);
            serviceManagerMock.Setup(m => m.VhostExists(It.IsAny<string>()));
            serviceManagerMock.Setup(m => m.CreateVhost(It.IsAny<string>()));
            serviceManagerMock.Setup(m => m.DropVhost(It.IsAny<string>()));
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .Build(new ConnectionParams{ HostName = "localhost" }, serviceManager: serviceManagerMock.Object);
            ctx.VhostName.Should().StartWith("evh");
        }
        
        [Fact]
        public void SetRequiredVhostName()
        {
            var serviceManagerMock = new Mock<IServiceManager>(MockBehavior.Strict);
            serviceManagerMock.Setup(m => m.VhostExists(It.IsAny<string>())).Returns(false);
            serviceManagerMock.Setup(m => m.CreateVhost(It.IsAny<string>()));
            serviceManagerMock.Setup(m => m.DropVhost(It.IsAny<string>()));
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .Build(new ConnectionParams{ HostName = "localhost" }, vhost: "prod", serviceManager: serviceManagerMock.Object);
            ctx.VhostName.Should().Be("prod");
        }
        
        [Fact]
        public void CreateEphemeralVhost()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .Build(_connectionParams);
            ctx.GetAllVhosts().Should().Contain(ctx.VhostName);
        }

    }
}