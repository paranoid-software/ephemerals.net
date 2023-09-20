using StackExchange.Redis;

namespace paranoid.software.ephemerals.Redis
{
    public interface IDbManager
    {

        int LockDatabase();
        RedisResult ExecuteScript(string sentence);
        long KeysCount();
        void ReleaseDatabase();
        
    }
}