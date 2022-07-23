using System.Collections.Generic;

namespace paranoid.software.ephemerals.MsSql
{
    public interface IDbManager
    {
        void CreateDatabase(string name);
        void ExecuteNonQuery(string sentence, string at);
        IEnumerable<Dictionary<string, object>> ExecuteQuery(string sentence, string at);
        void DropDatabase(string name);
    }
}