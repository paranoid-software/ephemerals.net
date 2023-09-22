using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace paranoid.software.ephemerals.RabbitMQ
{
    public interface IFilesManager
    {
        IEnumerable<JsonNode> ReadJsonArray(string filepath);
    }
}