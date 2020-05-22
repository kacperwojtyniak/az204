using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace az204api.Models
{
    public class Config
    {
        public string BlobUrl { get; set; }
        public string KeyVaultUrl { get; set; }
        public string ConnectionString { get; set; }
    }
}
