using System;
using FluentAssertions;
using paranoid.software.ephemerals.RabbitMQ;
using Xunit;

namespace tests
{
    public class CreatingAVirtualHostShould
    {
        [Fact]
        public void FailWhenHostNameIsNotAvailable()
        {
            var sut = new ServiceManager(new ConnectionParams { HostName = "localhost", AdminPortNumber = 15672, Username = "guest", Password = "guest"});
            var exception = Assert.Throws<Exception>(() => sut.CreateVhost("/"));
            exception.Should().BeNull();
        }
    }
}