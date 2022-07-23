using FluentAssertions;
using Moq;
using paranoid.software.ephemerals.PostgreSql;
using Xunit;

namespace tests
{
    public class BuildingContextShould
    {
        [Fact]
        public void SetEphemeralDbNamePrefix()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("");
            using var sut = new EphemeralPostgreSqlDbContextBuilder(filesManagerMock.Object)
                .Build("Server=localhost;Port=35432;", dbManagerMock.Object);
            sut.DbName.Should().StartWith("edb");
        }
        
        [Fact]
        public void ExecuteOneCommandPerAddedScript()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.CreateDatabase(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<string>()));
            dbManagerMock.Setup(m => m.DropDatabase(It.IsAny<string>()));
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns("CREATE TABLE test (id INT);");
            using var sut = new EphemeralPostgreSqlDbContextBuilder(filesManagerMock.Object)
                .AddScriptFromFile("script.sql")
                .AddScript("INSERT INTO test values(1);")
                .AddScript("INSERT INTO test values(2);")
                .AddScript("INSERT INTO test values(3);")
                .Build("Server=localhost;Port=35432;", dbManagerMock.Object);
            var dbName = sut.DbName;
            dbManagerMock.Verify(m => m.ExecuteNonQuery(It.IsAny<string>(), dbName), Times.Exactly(4));
        }

        [Fact]
        public void CreateTestTable()
        {
            using var sut = new EphemeralPostgreSqlDbContextBuilder()
                .AddScript("CREATE TABLE test (id INT);")
                .Build("Server=localhost;Port=35432;User Id=postgres;Password=postgres;");
            sut.GetAllTableNames().Should().Contain("test");
        }
        
        [Fact]
        public void Create2TestTableRows()
        {
            using var sut = new EphemeralPostgreSqlDbContextBuilder()
                .AddScript("CREATE TABLE test (id INT);")
                .AddScript("INSERT INTO test VALUES(1);")
                .AddScript("INSERT INTO test VALUES(2);")
                .Build("Server=localhost;Port=35432;User Id=postgres;Password=postgres;");
            sut.GetRowCount("test").Should().Be(2);
        }
        
        [Fact]
        public void CreateEphemeralDatabase()
        {
            using var sut = new EphemeralPostgreSqlDbContextBuilder()
                .Build("Server=localhost;Port=35432;User Id=postgres;Password=postgres;");
            sut.GetAllDatabaseNames().Should().Contain(sut.DbName);
        }
    }
}