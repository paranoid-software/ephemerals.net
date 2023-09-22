using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace paranoid.software.ephemerals.Redis
{
    
    internal class RedisDbPool: IDisposable
    {
        
        private readonly ConnectionMultiplexer _redis;
        private readonly HashSet<int> _availableDbs;

        public RedisDbPool(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _availableDbs = new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        }
        
        public IDatabase GetDatabase()
        {
            lock (_availableDbs)
            {
                if (_availableDbs.Count == 0)
                {
                    throw new InvalidOperationException("All databases are in use.");
                }

                // Get an available DB number
                using var enumerator = _availableDbs.GetEnumerator();
                enumerator.MoveNext(); // Advance to the first element
                var dbNumber = enumerator.Current;

                // Mark the DB as in use
                _availableDbs.Remove(dbNumber);

                return _redis.GetDatabase(dbNumber);
            }
        }
        
        public void ReleaseDatabase(IDatabase db)
        {
            if (db == null) return;

            lock (_availableDbs)
            {
                _availableDbs.Add(db.Database);
            }
            
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
        
    }
    
}
