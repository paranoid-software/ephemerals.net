using System;
using FluentAssertions;
using paranoid.software.ephemerals.Redis;
using Xunit;

namespace tests
{
    public class InitializingContextBuilderShould
    {
        [Fact]
        public void ThrowExceptionWhenHostNameIsNotLocal()
        {
            var exception = Assert.Throws<Exception>(() => new EphemeralRedisDbContextBuilder().Build("10.193.40.1"));
            exception.Message.Should().Be("Ephemeral database must be local, use localhost or 127.0.0.1 as host name.");
        }
    }
}