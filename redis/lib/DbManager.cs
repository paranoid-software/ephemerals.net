using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace paranoid.software.ephemerals.Redis
{
    internal class DbManager: IDbManager
    {
        
        private static RedisDbPool _redisDbPool;
        private IDatabase _db;

        private readonly List<string> _supportedCommands = new List<string>
        {
            "SET",
            "MSET",
            "SETEX",
            "SETNX",
            "APPEND",
            "LPUSH",
            "RPUSH",
            "LINSERT",
            "SADD",
            "ZADD",
            "HSET",
            "HMSET"
        };
        
        public DbManager(string connectionString)
        {
            var connectionStringParts = connectionString.Split(",").ToList();
            if (!connectionStringParts.Contains("allowAdmin=true")) connectionStringParts.Add("allowAdmin=true");
            _redisDbPool ??= new RedisDbPool(string.Join(",", connectionStringParts));
        }
        
        public int LockDatabase()
        {
            _db = _redisDbPool.GetDatabase();
            return _db.Database;
        }
        
        public RedisResult ExecuteScript(string sentence)
        {
            var parts = sentence.Split(' ');
            var command = parts[0];
            if (!_supportedCommands.Contains(command)) throw new ArgumentException($"Command not supported: {command}");
            var args = new object[parts.Length - 1];
            for (var i = 1; i < parts.Length; i++)
            {
                args[i - 1] = parts[i];
            }
            return _db.Execute(command, args);
        }

        public long KeysCount()
        {
            return _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First()).DatabaseSize(_db.Database);
        }
        
        public void ReleaseDatabase()
        {
            _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First()).FlushDatabase(_db.Database);
            _redisDbPool.ReleaseDatabase(_db);
        }

    }
}