using System.Collections.Generic;

namespace paranoid.software.ephemerals.Redis
{
    public interface IEphemeralRedisDbContextBuilder
    {
        IEphemeralRedisDbContextBuilder AddScriptSentence(string sentence);
        IEphemeralRedisDbContextBuilder AddScriptSentences(List<string> sentences);
        IEphemeralRedisDbContextBuilder AddScriptSentencesFromFile(string filepath);
        IEphemeralRedisDbContext Build(string connectionString, IDbManager dbManager = null);
    }
}