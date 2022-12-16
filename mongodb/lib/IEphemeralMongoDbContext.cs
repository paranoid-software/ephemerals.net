using System;
using System.Collections.Generic;

namespace paranoid.software.ephemerals.MongoDB
{
    public interface IEphemeralMongoDbContext: IDisposable
    {
        string DbName { get; }
        List<Exception> InitializationErrors { get; }
        IEnumerable<string> GetDatabaseNames();
        IEnumerable<string> GetCollectionNames();
        long GetDocumentsQty(string collName);
    }
}