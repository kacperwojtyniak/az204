using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Az204api.Models
{
    public class Config
    {
        public string BlobUrl { get; set; }
        public string KeyVaultUrl { get; set; }
        public string ConnectionString { get; set; }
        public string EventGridKey { get; set; }
        public string SecretKey { get; set; }
        public string EventGridUrl { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
