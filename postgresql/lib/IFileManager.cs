namespace paranoid.software.ephemerals.PostgreSql
{
    public interface IFilesManager
    {
        string ReadAllText(string filePath);
    }
}