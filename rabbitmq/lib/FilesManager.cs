using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

[assembly: InternalsVisibleTo("tests")]

namespace paranoid.software.ephemerals.RabbitMQ
{
    
    internal class FilesManager : IFilesManager
    {
        public IEnumerable<JsonNode> ReadJsonArray(string filepath)
        {
            try
            {
                var jsonString = File.ReadAllText(filepath);
                using var doc = JsonDocument.Parse(jsonString);
                var rootElement = doc.RootElement;
                if (rootElement.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonException("The JSON document is not an array.");
                }
                return rootElement.EnumerateArray().Select(item => JsonNode.Parse(item.GetRawText())).ToList();
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"The file at {filepath} could not be found.");
            }
            catch (JsonException)
            {
                throw new JsonException($"The content of the file at {filepath} is not a valid JSON array.");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while reading the file at {filepath}: {ex.Message}");
            }
        }
    }
}