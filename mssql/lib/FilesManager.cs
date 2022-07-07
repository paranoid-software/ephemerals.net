using System.IO;

namespace paranoid.software.ephemerals.MsSql
{
    
    public class FilesManager: IFilesManager
    {
        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}