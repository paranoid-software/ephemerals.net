using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using paranoid.software.ephemerals.Redis;
using StackExchange.Redis;
using Xunit;

namespace tests
{
    public class LeavingContextShould
    {
        [Fact]
        public void ReleaseDatabase()
        {   
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.LockDatabase()).Returns(0);
            dbManagerMock.Setup(m => m.ExecuteScript(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.ReleaseDatabase());
            using (var ctx = new EphemeralRedisDbContextBuilder()
                       .AddScriptSentence("SET my-kye my-value")
                       .Build("localhost,password=pwd", dbManagerMock.Object))
            {
                
            }
            dbManagerMock.Verify(m => m.ReleaseDatabase(), Times.Exactly(1));
        }
    }
}