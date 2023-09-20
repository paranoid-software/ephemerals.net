using System;
using System.Collections.Generic;

namespace paranoid.software.ephemerals.Redis
{
    public interface IEphemeralRedisDbContext: IDisposable
    {
        int DatabaseNumber { get; }
        List<Exception> InitializationErrors { get; }
        long KeysCount();
    }
}