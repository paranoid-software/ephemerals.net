using System;
using FluentAssertions;
using paranoid.software.ephemerals.MsSql;
using Xunit;

namespace tests
{

    public class InitializingContextShould
    {
        [Fact]
        public void ThrowExceptionWhenDataSourceIsNotLocal()
        {
            var exception = Assert.Throws<Exception>(() =>
                new EphemeralMsSqlDbContext("Data Source=192.168.1.1,31433;Persist Security Info=False;Max Pool Size=1024;"));
            exception.Message.Should().Be("Ephemeral database server must be local, use localhost or 127.0.0.1 as server address.");
        }

        [Fact]
        public void ThrowExceptionWhenDatabaseNameIsPresent()
        {
            var exception = Assert.Throws<Exception>(() =>
                new EphemeralMsSqlDbContext("Data Source=localhost,31433;Persist Security Info=False;Max Pool Size=1024;Initial Catalog=my_database;"));
            exception.Message.Should().Be("Ephemeral database name should not be included on the connection string, please remove Initial Catalog parameter.");
        }
    }
}