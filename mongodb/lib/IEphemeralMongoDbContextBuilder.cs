using System.Collections.Generic;

namespace paranoid.software.ephemerals.MongoDB
{
    public interface IEphemeralMongoDbContextBuilder
    {
        IEphemeralMongoDbContextBuilder AddItemsFromFile(string filepath);
        IEphemeralMongoDbContextBuilder AddItems(string collName, IEnumerable<dynamic> items);
        IEphemeralMongoDbContext Build(ConnectionParams connectionParams, string dbName = null, IDbManager dbManager = null);
    }
}