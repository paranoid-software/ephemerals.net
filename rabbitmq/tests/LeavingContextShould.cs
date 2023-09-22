using FluentAssertions;
using paranoid.software.ephemerals.RabbitMQ;
using Xunit;

namespace tests
{
    public class LeavingContextShould
    {
        [Fact]
        public void DropEphemeralVhost()
        {
            var connectionParams = new ConnectionParams
            {
                HostName = "localhost",
                ManagementPortNumber = 15672,
                Username = "guest",
                Password = "guest",
                SslEnabled = false
            };
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .Build(connectionParams);
            using (var ctx2 = new EphemeralRabbitMqContextBuilder()
                       .Build(connectionParams, "my-vhost"))
            {
                ctx2.GetAllVhosts().Should().Contain("my-vhost");        
            }
            ctx.GetAllVhosts().Should().NotContain("my-vhost");
        }
    }
}