using FluentAssertions;
using paranoid.software.ephemerals.MongoDB;
using Xunit;

namespace tests
{
    public class LeavingContextShould
    {
        [Fact]
        public void DropEphemeralDatabase()
        {
            using var ctx = new EphemeralMongoDbContextBuilder()
                .Build(new ConnectionParams{ HostName = "localhost", PortNumber = 27017, Username = "root", Password = "pwd", IsDirectConnection = true });
            using (var ctx2 = new EphemeralMongoDbContextBuilder()
                       .Build(new ConnectionParams { HostName = "localhost", PortNumber = 27017, Username = "root", Password = "pwd", IsDirectConnection = true }, "my_db"))
            {
                ctx2.GetDatabaseNames().Should().Contain("my_db");
            }
            ctx.GetDatabaseNames().Should().NotContain("my_db");
        }
    }
}