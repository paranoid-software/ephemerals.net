using System;

namespace paranoid.software.ephemerals.RabbitMQ
{
    
    public class ConnectionParams
    {
        public string HostName { get; set; }
        public int PortNumber { get; set; }
        public int ManagementPortNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool SslEnabled { get; set; }

        public Uri GetManagementBaseUri()
        {
            return new Uri($"{(SslEnabled ? "https://" : "http://")}{HostName}:{ManagementPortNumber}");
        }

        public Uri GetAmqpUri()
        {
            return new Uri($"{(SslEnabled ? "amqps://" : "amqp://")}{Username}:{Password}@{HostName}:{PortNumber}");
        }
        
    }
    
}