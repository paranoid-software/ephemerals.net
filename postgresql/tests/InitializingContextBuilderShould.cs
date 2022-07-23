using System;
using FluentAssertions;
using paranoid.software.ephemerals.PostgreSql;
using Xunit;

namespace tests
{

    public class InitializingContextBuilderShould
    {
        [Fact]
        public void ThrowExceptionWhenDataSourceIsNotLocal()
        {
            var exception = Assert.Throws<Exception>(() =>
                new EphemeralPostgreSqlDbContextBuilder().Build("Server=192.168.1.1;Port=35432;"));
            exception.Message.Should().Be("Ephemeral database server must be local, use localhost or 127.0.0.1 as server address.");
        }

        [Fact]
        public void ThrowExceptionWhenDatabaseNameIsPresent()
        {
            var exception = Assert.Throws<Exception>(() =>
                new EphemeralPostgreSqlDbContextBuilder().Build("Server=localhost;Port=35432;Database=my_database;"));
            exception.Message.Should().Be("Ephemeral database name should not be included on the connection string, please remove Initial Catalog parameter.");
        }
    }
}