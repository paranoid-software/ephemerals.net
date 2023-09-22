using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using paranoid.software.ephemerals.RabbitMQ;

namespace tests
{
    public class AddingDirectQueueMessagesShould
    {
        private readonly ConnectionParams _connectionParams = new ConnectionParams
        {
            HostName = "localhost",
            ManagementPortNumber = 15672,
            Username = "guest",
            Password = "guest",
            SslEnabled = false
        };
        
        [Fact]
        public void Post4MessagesToTheGivenQueue()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .AddQueue("books")
                .AddDirectQueueMessages("books", new List<string> {"Hello", "World !"})
                .AddDirectQueueMessagesFromFile("books", "test-data/json-array-with-2-items.json")
                .Build(_connectionParams);
            var result = ctx.GetMessagesFromQueue("books", 10);
            result.Count().Should().Be(4);
        }

        [Fact]
        public void FailWhenQueueDoesNotExist()
        {
            using var ctx = new EphemeralRabbitMqContextBuilder()
                .AddDirectQueueMessage("books", "A simple message")
                .Build(_connectionParams);
            ctx.InitializationErrors.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ThrowExceptionWhenMessagesFilesDoesNotExist()
        {
            var exception = Assert.Throws<FileNotFoundException>(() => 
                new EphemeralRabbitMqContextBuilder()
                    .AddDirectQueueMessagesFromFile("books", "test-data/missing-file.json")
                    .Build(_connectionParams));
            exception.Message.Should().Be("The file at test-data/missing-file.json could not be found.");
        }
        
        [Fact]
        public void ThrowExceptionWhenMessagesFilesContentIsInvalid()
        {
            var exception = Assert.Throws<JsonException>(() => 
                new EphemeralRabbitMqContextBuilder()
                    .AddDirectQueueMessagesFromFile("books", "test-data/invalid-content.json")
                    .Build(_connectionParams));
            exception.Message.Should().Be("The content of the file at test-data/invalid-content.json is not a valid JSON array.");
        }
        
    }
}