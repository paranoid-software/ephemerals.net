using System.Collections.Generic;

namespace paranoid.software.ephemerals.MsSql
{
    public class EphemeralMsSqlDbContextBuilder : IEphemeralMsSqlDbContextBuilder
    {

        private readonly IFilesManager _filesManager;
        private readonly List<string> _scripts;
        
        public EphemeralMsSqlDbContextBuilder() : this(new FilesManager())
        {
            _scripts = new List<string>();
        }

        public EphemeralMsSqlDbContextBuilder(IFilesManager filesManager)
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
            return new EphemeralDbContext(serverConnectionString, _scripts, new DbManager(serverConnectionString));
        }

        public IEphemeralMsSqlDbContext Build(string serverConnectionString, IDbManager dbManager)
        {
            return new EphemeralDbContext(serverConnectionString, _scripts, dbManager);
        }

    }
}