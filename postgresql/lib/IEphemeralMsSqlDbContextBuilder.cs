
namespace paranoid.software.ephemerals.PostgreSql
{
    public interface IEphemeralMsSqlDbContextBuilder
    {
        IEphemeralMsSqlDbContextBuilder AddScriptFromFile(string filePath);
        IEphemeralMsSqlDbContextBuilder AddScript(string sentence);
        IEphemeralMsSqlDbContext Build(string serverConnectionString);
        IEphemeralMsSqlDbContext Build(string serverConnectionString, IDbManager dbManager);
    }
}