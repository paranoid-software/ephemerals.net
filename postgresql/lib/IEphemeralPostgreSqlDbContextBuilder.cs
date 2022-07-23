namespace paranoid.software.ephemerals.PostgreSql
{
    public interface IEphemeralPostgreSqlDbContextBuilder
    {
        IEphemeralPostgreSqlDbContextBuilder AddScriptFromFile(string filePath);
        IEphemeralPostgreSqlDbContextBuilder AddScript(string sentence);
        IEphemeralPostgreSqlDbContext Build(string serverConnectionString);
        IEphemeralPostgreSqlDbContext Build(string serverConnectionString, IDbManager dbManager);
    }
}