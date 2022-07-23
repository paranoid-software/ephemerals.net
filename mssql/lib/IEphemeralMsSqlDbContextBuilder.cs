namespace paranoid.software.ephemerals.MsSql
{
    public interface IEphemeralMsSqlDbContextBuilder
    {
        IEphemeralMsSqlDbContextBuilder AddScriptFromFile(string filePath);
        IEphemeralMsSqlDbContextBuilder AddScript(string sentence);
        IEphemeralMsSqlDbContext Build(string serverConnectionString);
        IEphemeralMsSqlDbContext Build(string serverConnectionString, IDbManager dbManager);
    }
}