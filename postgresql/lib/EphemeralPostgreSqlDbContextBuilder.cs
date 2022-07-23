using System.Collections.Generic;

namespace paranoid.software.ephemerals.PostgreSql
{
    public class EphemeralPostgreSqlDbContextBuilder : IEphemeralMsSqlDbContextBuilder
    {

        private readonly IFilesManager _filesManager;
        private readonly List<string> _scripts;
        
        public EphemeralPostgreSqlDbContextBuilder() : this(new FilesManager())
        {
            _scripts = new List<string>();
        }

        public EphemeralPostgreSqlDbContextBuilder(IFilesManager filesManager)
        {
            _filesManager = filesManager;
            _scripts = new List<string>();
        }
        
        public IEphemeralMsSqlDbContextBuilder AddScriptFromFile(string filePath)
        {
            _scripts.Add(_filesManager.ReadAllText(filePath));
            return this;
        }

        public IEphemeralMsSqlDbContextBuilder AddScript(string sentence)
        {
            _scripts.Add(sentence);
            return this;
        }
        
        public IEphemeralMsSqlDbContext Build(string serverConnectionString)
        {
            return new EphemeralPostgreSqlDbContext(serverConnectionString, _scripts, new DbManager(serverConnectionString));
        }

        public IEphemeralMsSqlDbContext Build(string serverConnectionString, IDbManager dbManager)
        {
            return new EphemeralPostgreSqlDbContext(serverConnectionString, _scripts, dbManager);
        }

    }
}