using System;
using System.Collections.Generic;
using paranoid.software.ephemerals.MsSql.paranoid.software.ephemerals.MsSql;

namespace paranoid.software.ephemerals.MsSql
{
    public interface IEphemeralMsSqlDbContextBuilder: IDisposable
    {
        IEphemeralMsSqlDbContextBuilder AddScriptFromFile(string filePath);
        IEphemeralMsSqlDbContextBuilder AddScript(string sentence);
        IEphemeralMsSqlDbContext Build();
    }
}