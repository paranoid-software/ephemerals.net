using System.Collections.Generic;

namespace paranoid.software.ephemerals.Redis
{
    public interface IFilesManager
    {
        IEnumerable<string> ReadLines(string filePath);
    }
}