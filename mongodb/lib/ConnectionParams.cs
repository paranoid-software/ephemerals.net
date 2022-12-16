namespace paranoid.software.ephemerals.MongoDB
{
    public class ConnectionParams
    {
        public string HostName { get; set; }
        public int PortNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsDirectConnection { get; set; }

        public string GetConnectionString()
        {
            return string.Format(
                $"mongodb://{Username}:{Password}@{HostName}:{PortNumber}/?directConnection={IsDirectConnection.ToString().ToLower()}");
        }
    }
}