using System;
using System.Collections.Generic;

namespace paranoid.software.ephemerals.PostgreSql
{
    public interface IEphemeralMsSqlDbContext : IDisposable
    {
        string DbName { get; }
        List<Exception> ScriptsErrors { get; }
        IEnumerable<string> GetAllDatabaseNames();
        IEnumerable<string> GetAllTableNames();
        int GetRowCount(string tableName);
    }
}