using System;
using System.Collections.Generic;
using System.Linq;

namespace paranoid.software.ephemerals.Redis
{
    public class EphemeralRedisDbContextBuilder: IEphemeralRedisDbContextBuilder
    {
        
        private readonly IFilesManager _filesManager;

        private readonly List<string> _scriptSentences;
        
        public EphemeralRedisDbContextBuilder() : this(new FilesManager())
        {
            _scriptSentences = new List<string>();
        }
        
        public EphemeralRedisDbContextBuilder(IFilesManager filesManager)
        {
            _filesManager = filesManager;
            _scriptSentences = new List<string>();
        }

        public IEphemeralRedisDbContextBuilder AddScriptSentence(string sentence)
        {
            _scriptSentences.Add(sentence);
            return this;
        }

        public IEphemeralRedisDbContextBuilder AddScriptSentences(IEnumerable<string> sentences)
        {
            _scriptSentences.AddRange(sentences);
            return this;
        }

        public IEphemeralRedisDbContextBuilder AddScriptSentencesFromFile(string filepath)
        {
            var scriptSentences = _filesManager.ReadLines(filepath);
            foreach (var sentence in scriptSentences)
            {
                _scriptSentences.Add(sentence);
            }
            return this;
        }
        
        public IEphemeralRedisDbContext Build(string connectionString, IDbManager dbManager = null)
        {
            var supportedHostNames = new[] { "localhost", "127.0.0.1" };
            var isValidHost = supportedHostNames.Any(connectionString.Contains);
            if (!isValidHost)
            {
                throw new Exception("Ephemeral database must be local, use localhost or 127.0.0.1 as host name.");
            }
            return new EphemeralRedisDbContext(dbManager ?? new DbManager(connectionString), _scriptSentences);
        }
    }
}