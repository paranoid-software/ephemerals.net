using System;
using FluentAssertions;
using paranoid.software.ephemerals.MongoDB;
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
                PortNumber = 27017, 
                Username = "root",
                Password = "pwd",
                IsDirectConnection = true
            };
            var exception = Assert.Throws<Exception>(() => new EphemeralMongoDbContextBuilder().Build(connectionParams));
            exception.Message.Should().Be("Ephemeral database must be local, use localhost or 127.0.0.1 as host name.");
        }
        
        [Fact]
        public void ThrowExceptionWhenDbNameIsNotAllowed()
        {
            var connectionParams = new ConnectionParams
            {
                HostName = "localhost", 
                PortNumber = 27017, 
                Username = "root",
                Password = "pwd",
                IsDirectConnection = true
            };
            var exception = Assert.Throws<Exception>(() => new EphemeralMongoDbContextBuilder().Build(connectionParams, "admin"));
            exception.Message.Should().Be("Database name admin is not allowed !");
        }
        
    }
}