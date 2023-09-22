using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Moq;
using paranoid.software.ephemerals.MongoDB;
using Xunit;

namespace tests
{
    public class BuildingContextShould
    {
        [Fact]
        public void ThrowExceptionWhenDbNameIsNotAvailable()
        {
            var connectionParams = new ConnectionParams
            {
                HostName = "localhost",
                PortNumber = 27017,
                Username = "root",
                Password = "pwd",
                IsDirectConnection = true
            };
            using var ctx1 = new EphemeralMongoDbContextBuilder().Build(connectionParams, "my_db_name");
            var exception = Assert.Throws<Exception>(() =>
            {
                using var ctx2 = new EphemeralMongoDbContextBuilder().Build(connectionParams, "my_db_name");
            });
            exception.Message.Should().Be("Database name my_db_name is not available !");
        }

        [Fact]
        public void SetEphemeralDbNamePrefix()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.DatabaseExists(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("");
            using var ctx = new EphemeralMongoDbContextBuilder(filesManagerMock.Object)
                .Build(new ConnectionParams{ HostName = "localhost" }, dbManager: dbManagerMock.Object);
            ctx.DbName.Should().StartWith("edb");
        }
        
        [Fact]
        public void SetRequiredDbName()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.DatabaseExists(It.IsAny<string>())).Returns(false);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("");
            using var ctx = new EphemeralMongoDbContextBuilder(filesManagerMock.Object)
                .Build(new ConnectionParams{ HostName = "localhost" }, dbName: "my_db_name", dbManager: dbManagerMock.Object);
            ctx.DbName.Should().Be("my_db_name");
        }

        [Fact]
        public void ThrowExceptionWhenDataFileIsInvalid()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.DatabaseExists(It.IsAny<string>())).Returns(false);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("This is not JSON content expected.");
            var exception = Assert.Throws<Exception>(() =>
            {
                using var ctx = new EphemeralMongoDbContextBuilder(filesManagerMock.Object)
                    .AddItemsFromFile("books.json")
                    .Build(new ConnectionParams{ HostName = "localhost" }, dbName: "my_db_name", dbManager: dbManagerMock.Object);
            });
            exception.Message.Should().Be($"File content for books.json is not valid !");
        }

        [Fact]
        public void ExecuteOneInsertPerAddedDocument()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.DatabaseExists(It.IsAny<string>())).Returns(false);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns(JsonSerializer.Serialize(new Dictionary<string, List<dynamic>>
            {
                {"users", new List<dynamic> { new {username= "test"} }}
            }));
            using var ctx = new EphemeralMongoDbContextBuilder(filesManagerMock.Object)
                .AddItemsFromFile("books.json")
                .AddItems("books", new List<dynamic>
                {
                    new {name="Pinocchio"},
                    new {name="El Quixote"}
                })
                .Build(new ConnectionParams{ HostName = "localhost" }, dbName: "my_db_name", dbManager: dbManagerMock.Object);
            dbManagerMock.Verify(m => m.InsertDocument(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
        }

        [Fact]
        public void CreateBooksCollection()
        {
            using var ctx = new EphemeralMongoDbContextBuilder()
                .AddItems("books", new List<dynamic>
                {
                    new {name="Pinocchio"},
                    new {name="El Quixote"}
                })
                .Build(new ConnectionParams{ HostName = "localhost", PortNumber = 27017, Username = "root", Password = "pwd", IsDirectConnection = true}, dbName: "my_db_name");
            ctx.GetCollectionNames().Should().Contain("books");
        }
        
        [Fact]
        public void Create1ItemAtBooksCollection()
        {
            var connectionParams = new ConnectionParams
            {
                HostName = "localhost",
                PortNumber = 27017,
                Username = "root",
                Password = "pwd",
                IsDirectConnection = true
            };
            using var ctx = new EphemeralMongoDbContextBuilder()
                .AddItems("books", new List<dynamic>
                {
                    new {name="Pinocchio"}
                })
                .Build(connectionParams, "my_db_name");
            ctx.GetDocumentsQty("books").Should().Be(1);
        }

        [Fact]
        public void CreateEphemeralDatabase()
        {
            using var ctx = new EphemeralMongoDbContextBuilder()
                .AddItems("books", new List<dynamic>
                {
                    new {name="Pinocchio"}
                })
                .Build(new ConnectionParams{ HostName = "localhost", PortNumber = 27017, Username = "root", Password = "pwd", IsDirectConnection = true});
            ctx.GetDatabaseNames().Should().Contain(ctx.DbName);
        }
    }
}