using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using paranoid.software.ephemerals.Redis;
using Xunit;

namespace tests
{
    public class BuildingContextShould
    {
        
        [Fact]
        public void ExecuteOneCommandScriptSentence()
        {
            var dbManagerMock = new Mock<IDbManager>(MockBehavior.Strict);
            dbManagerMock.Setup(m => m.LockDatabase()).Returns(0);
            dbManagerMock.Setup(m => m.ExecuteScript(It.IsAny<string>()));
            dbManagerMock.Setup(m => m.ReleaseDatabase());
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new List<string>{ "SET my-cool-key my-extra-cool-value" });
            using var ctx = new EphemeralRedisDbContextBuilder(filesManagerMock.Object)
                .AddScriptSentencesFromFile("my-mocked-file.json")
                .AddScriptSentences(new List<string>
                {
                    "SET 1 Pinocchio",
                    "SET 2 El Quixote"
                })
                .Build("localhost", dbManager: dbManagerMock.Object);
            dbManagerMock.Verify(m => m.ExecuteScript(It.IsAny<string>()), Times.Exactly(3));
        }
        
        [Fact]
        public void ThrowArgumentExceptionWhenKeyValuePairsListFileIsInvalid()
        {
            var filesManagerMock = new Mock<IFilesManager>(MockBehavior.Strict);
            filesManagerMock.Setup(m => m.ReadLines(It.IsAny<string>())).Returns(new List<string> { "Some irrelevant text." });
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                using (var ctx = new EphemeralRedisDbContextBuilder(filesManagerMock.Object)
                    .AddScriptSentencesFromFile("my-mocked-file.json")
                    .Build("localhost,password=pwd"))
                {
                    
                }
            });
            exception.Message.Should().Be("Command not supported: Some");
        }
        
        [Fact]
        public void Add3KeysFromFile()
        {
            using var sut = new EphemeralRedisDbContextBuilder()
                .AddScriptSentences(new List<string>
                {
                    "SET 1 pablo",
                    "LPUSH 2 A B C",
                    "SADD 3 Quito Guayaquil Manta"
                })
                .Build("localhost,password=pwd");
            sut.KeysCount().Should().Be(3);
        }
        
    }
}