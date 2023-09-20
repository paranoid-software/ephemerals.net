using System.Collections.Generic;
using System.IO;

namespace paranoid.software.ephemerals.Redis
{
    public class FilesManager: IFilesManager
    {
        public IEnumerable<string> ReadLines(string filePath)
        {
            return File.ReadLines(filePath);
        }
    }
}