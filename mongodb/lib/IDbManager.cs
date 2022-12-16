using System.Collections.Generic;

namespace paranoid.software.ephemerals.MongoDB
{
    public interface IDbManager
    {
        bool DatabaseExists(string name);
        IEnumerable<string> GetDatabaseNames();
        void CreateDatabase(string name);
        IEnumerable<string> GetCollectionNames(string dbName);
        void InsertDocument(string dbName, string collName, object doc);
        long GetDocumentsQty(string dbName, string collName);
        void DropDatabase(string name);
    }
}