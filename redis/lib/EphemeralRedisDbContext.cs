using System;
using System.Collections.Generic;
using StackExchange.Redis;


namespace paranoid.software.ephemerals.Redis
{
    public class EphemeralRedisDbContext: IEphemeralRedisDbContext
    {
        
        private readonly IDbManager _dbManager;
        
        public int DatabaseNumber { get; private set; }
        public List<Exception> InitializationErrors { get; }
        
        public EphemeralRedisDbContext(IDbManager dbManager, List<string> scriptSentences)
        {
            _dbManager = dbManager;
            InitializationErrors = new List<Exception>();
            InitDbWith(scriptSentences);
        }

        private void InitDbWith(List<string> scriptSentences)
        {
            DatabaseNumber = _dbManager.LockDatabase();
            foreach (var sentence in scriptSentences)
            {
                try
                {
                    var result = _dbManager.ExecuteScript(sentence);
                    if (result.Type == ResultType.Error)
                    {
                        InitializationErrors.Add(new Exception(result.ToString()));
                    }
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    InitializationErrors.Add(ex);
                }
            }
        }
        
        public long KeysCount()
        {
            return _dbManager.KeysCount();
        }

        public void Dispose()
        {
            _dbManager.ReleaseDatabase();
        }
    }
}