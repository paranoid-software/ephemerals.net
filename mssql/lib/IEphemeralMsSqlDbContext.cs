using System;
using System.Collections.Generic;

namespace paranoid.software.ephemerals.MsSql
{
    public interface IEphemeralMsSqlDbContext: IDisposable
    {
        string DbName { get; }
        IEphemeralMsSqlDbContext AddScriptFromFile(string filePath);
        IEphemeralMsSqlDbContext AddScript(string sentence);
        IEphemeralMsSqlDbContext Build();
        IEnumerable<string> GetAllDatabaseNames();
        IEnumerable<string> GetAllTableNames();
        int GetRowCount(string tableName);
    }
}