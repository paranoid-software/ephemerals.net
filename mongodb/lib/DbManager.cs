using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace paranoid.software.ephemerals.MongoDB
{
    public class DbManager : IDbManager
    {
        private readonly ConnectionParams _connectionParams;
        
        public DbManager(ConnectionParams connectionParams)
        {
            _connectionParams = connectionParams;
        }

        public bool DatabaseExists(string name)
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            return client.ListDatabaseNames().ToList().Contains(name);
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            return client.ListDatabaseNames().ToList();
        }

        public void CreateDatabase(string name)
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            client.GetDatabase(name).GetCollection<dynamic>("dummy").InsertOne(new { hi = "stranger"});
        }

        public IEnumerable<string> GetCollectionNames(string dbName)
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            return client.GetDatabase(dbName).ListCollectionNames().ToList();
        }

        public void InsertDocument(string dbName, string collName, object doc)
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            client.GetDatabase(dbName).GetCollection<object>(collName).InsertOne(doc);
        }
        
        public long GetDocumentsQty(string dbName, string collName)
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            return client.GetDatabase(dbName).GetCollection<BsonDocument>(collName).CountDocuments(new BsonDocument());
        }

        public void DropDatabase(string name)
        {
            var client = new MongoClient(_connectionParams.GetConnectionString());
            client.DropDatabase(name);
        }
        
    }
}