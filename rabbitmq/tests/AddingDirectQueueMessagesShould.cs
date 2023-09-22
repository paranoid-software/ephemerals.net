using System.Linq;
using FluentAssertions;
using Xunit;
using paranoid.software.ephemerals.RabbitMQ;

namespace tests
{
    public class AddingDirectMessagesToQueueShould
    {
        [Fact]
        public void LoadListWith2Items()
        {
            var sut = new FilesManager();
            var result = sut.ReadJsonArray("test-data/json-array-with-2-items.json");
            result.Count().Should().Be(2);
        }
    }
}