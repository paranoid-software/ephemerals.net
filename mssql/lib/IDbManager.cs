using System.Collections.Generic;

namespace paranoid.software.ephemerals.MsSql
{
    public interface IDbManager
    {
        void CreateDatabase(string name);
        void ExecuteNonQuery(string sentence, string at);
        IEnumerable<string> GetAllDatabaseNames();
        void DropDatabase(string name);
    }
}