using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;

namespace paranoid.software.ephemerals.PostgreSql
{
    public class DbManager: IDbManager
    {
        private readonly string _serverConnectionString;

        public DbManager(string serverConnectionString)
        {
            _serverConnectionString = serverConnectionString;
        }
        public void CreateDatabase(string name)
        {
            using var serverConnection = new NpgsqlConnection($"{_serverConnectionString};");
            using var cmd = new NpgsqlCommand($"CREATE DATABASE {name};", serverConnection);
            serverConnection.Open();
            cmd.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string sentence, string at)
        {
            using var cnn = new NpgsqlConnection($"{_serverConnectionString};Database={at};");
            cnn.Open();
            using var sentenceCmd = new NpgsqlCommand(sentence, cnn);
            sentenceCmd.ExecuteNonQuery();
        }

        public IEnumerable<Dictionary<string, object>> ExecuteQuery(string sentence, string at)
        {
            using var cnn = new NpgsqlConnection($"{_serverConnectionString};Database={at};");
            cnn.Open();
            using var cmd = new NpgsqlCommand(sentence, cnn);
            using var reader = cmd.ExecuteReader();
            var columNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            foreach (IDataRecord record in reader)
            {
                var row = new Dictionary<string, object>();
                foreach (var columName in columNames)
                    row[columName] = record[columName];
                yield return row;
            }
        }
        
        public void DropDatabase(string name)
        {
            using var cnn = new NpgsqlConnection($"{_serverConnectionString};");
            cnn.Open();
            using var cmd =
                new NpgsqlCommand($"DROP DATABASE {name} WITH (FORCE);", cnn);
            cmd.ExecuteNonQuery();
        }

    }
}