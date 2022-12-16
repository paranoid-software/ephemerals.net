using System.IO;

namespace paranoid.software.ephemerals.MongoDB
{
    
    public class FilesManager: IFilesManager
    {
        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}