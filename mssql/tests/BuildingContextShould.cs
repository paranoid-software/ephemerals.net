using FluentAssertions;
using Moq;
using paranoid.software.ephemerals.MsSql;
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
            using var sut = new EphemeralMsSqlDbContextBuilder(filesManagerMock.Object)
                .Build("Data Source=localhost;Persist Security Info=False;Max Pool Size=1024;", dbManagerMock.Object);
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
            using var sut = new EphemeralMsSqlDbContextBuilder(filesManagerMock.Object)
                .AddScriptFromFile("script.sql")
                .AddScript("INSERT INTO test values(1);")
                .AddScript("INSERT INTO test values(2);")
                .AddScript("INSERT INTO test values(3);")
                .Build("Data Source=localhost;Persist Security Info=False;Max Pool Size=1024;", dbManagerMock.Object);
            var dbName = sut.DbName;
            dbManagerMock.Verify(m => m.ExecuteNonQuery(It.IsAny<string>(), dbName), Times.Exactly(4));
        }

        [Fact]
        public void CreateTestTable()
        {
            using var sut = new EphemeralMsSqlDbContextBuilder()
                .AddScript("CREATE TABLE test (id INT);")
                .Build("Data Source=localhost,31433;User Id=sa;Password=my-New-pwd;Persist Security Info=False;Max Pool Size=1024;");
            sut.GetAllTableNames().Should().Contain("test");
        }
        
        [Fact]
        public void Create2TestTableRows()
        {
            using var sut = new EphemeralMsSqlDbContextBuilder()
                .AddScript("CREATE TABLE test (id INT);")
                .AddScript("INSERT INTO test VALUES(1);")
                .AddScript("INSERT INTO test VALUES(2);")
                .Build("Data Source=localhost,31433;User Id=sa;Password=my-New-pwd;Persist Security Info=False;Max Pool Size=1024;");
            sut.GetRowCount("test").Should().Be(2);
        }
        
        [Fact]
        public void CreateEphemeralDatabase()
        {
            using var sut = new EphemeralMsSqlDbContextBuilder()
                .Build("Data Source=localhost,31433;User Id=sa;Password=my-New-pwd;Persist Security Info=False;Max Pool Size=1024;");
            sut.GetAllDatabaseNames().Should().Contain(sut.DbName);
        }
    }
}