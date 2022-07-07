using System;
using FluentAssertions;
using Moq;
using paranoid.software.ephemerals.MsSql;
using Xunit;

namespace tests
{
    public class BuildingEphemeralDbContextShould
    {
        [Fact]
        public void SetRandomDbName()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("");
            using var sut = new EphemeralMsSqlDbContext("Data Source=localhost;Persist Security Info=False;Max Pool Size=1024;", dbManagerMock.Object, filesManagerMock.Object)
                .Build();
            sut.DbName.Should().NotBeNull();
        }
        
        [Fact]
        public void SetDeclaredDbName()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("");
            using var sut = new EphemeralMsSqlDbContext("Data Source=localhost;Persist Security Info=False;Max Pool Size=1024;", dbManagerMock.Object, filesManagerMock.Object)
                .SetDatabaseName("test")
                .Build();
            sut.DbName.Should().NotBeNull();
            sut.DbName.Should().Be("test");
        }
        
        [Fact]
        public void ExecuteOneCommandPerEveryAddedScript()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("");
            using var sut = new EphemeralMsSqlDbContext("Data Source=localhost;Persist Security Info=False;Max Pool Size=1024;", dbManagerMock.Object, filesManagerMock.Object)
                .AddScriptFromFile("script.sql")
                .AddScript("")
                .AddScript("")
                .AddScript("")
                .Build();
            var dbName = sut.DbName;
            dbManagerMock.Verify(m => m.ExecuteNonQuery(It.IsAny<string>(), dbName), Times.Exactly(4));
        }

        [Fact]
        public void ThrowExceptionWhenDbNameFormatIsInvalid()
        {
            var exception = Assert.Throws<Exception>(() =>
                new EphemeralMsSqlDbContext("Data Source=localhost,31433;User Id=sa;Password=my-New-pwd;Persist Security Info=False;Max Pool Size=1024;")
                    .SetDatabaseName("my_invalid-db-name")
                    .Build());
            exception.Message.Should().Be("Database name is invalid.");
        }

        [Fact]
        public void CreateNewDatabase()
        {
            using var sut = new EphemeralMsSqlDbContext("Data Source=localhost,31433;User Id=sa;Password=my-New-pwd;Persist Security Info=False;Max Pool Size=1024;")
                .Build();
            sut.GetAllDatabaseNames().Should().Contain(sut.DbName);
        }
    }
}