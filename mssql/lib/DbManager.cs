using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace paranoid.software.ephemerals.MsSql
{
	
    public class DbManager: IDbManager
    {
        private static string _serverConnectionString;

        public DbManager(string serverConnectionString)
        {
            _serverConnectionString = serverConnectionString;
        }
        public void CreateDatabase(string name)
        {
            if (!new Regex(@"^\w+$").IsMatch(name)) throw new Exception("Database name is invalid.");
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

        public IEnumerable<string> GetAllDatabaseNames()
        {
            using var cnn = new SqlConnection($"{_serverConnectionString};Initial Catalog=master;");
            cnn.Open();
            using var cmd = new SqlCommand("SELECT name FROM master.sys.databases;", cnn);
            var result = new List<string>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            return result;
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