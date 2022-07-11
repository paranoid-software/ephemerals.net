using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace paranoid.software.ephemerals.MsSql
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
            using var serverConnection = new SqlConnection($"{_serverConnectionString};Initial Catalog=master");
            using var cmd = new SqlCommand($"CREATE DATABASE [{name}];", serverConnection);
            serverConnection.Open();
            cmd.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string sentence, string at)
        {
            using var cnn = new SqlConnection($"{_serverConnectionString};Initial Catalog={at};");
            cnn.Open();
            using var sentenceCmd = new SqlCommand(sentence, cnn);
            sentenceCmd.ExecuteNonQuery();
        }

        public IEnumerable<Dictionary<string, object>> ExecuteQuery(string sentence, string at)
        {
            using var cnn = new SqlConnection($"{_serverConnectionString};Initial Catalog={at};");
            cnn.Open();
            using var cmd = new SqlCommand(sentence, cnn);
            using var reader = cmd.ExecuteReader();
            var columNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            foreach (IDataRecord record in (IEnumerable)reader)
            {
                var row = new Dictionary<string, object>();
                foreach (var columName in columNames)
                    row[columName] = record[columName];
                yield return row;
            }
        }
        
        public void DropDatabase(string name)
        {
            using var cnn = new SqlConnection($"{_serverConnectionString};Initial Catalog=master");
            cnn.Open();
            using var cmd =
                new SqlCommand($"ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}];", cnn);
            cmd.ExecuteNonQuery();
        }

    }
}