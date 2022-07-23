using System.IO;

namespace paranoid.software.ephemerals.PostgreSql
{
    
    public class FilesManager: IFilesManager
    {
        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}