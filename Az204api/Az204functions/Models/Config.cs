using System;
using System.Collections.Generic;
using System.Text;

namespace Az204functions.Models
{
    public class Config
    {
        public string DatabaseName { get; set; }
        public string InputCollection { get; set; }
        public string ConnectionString { get; set; }
        public string OutputCollectionBrewing { get; set; }
        public string OutputCollectionOrigin { get; set; }
        public string OrdersCollection { get; set; }
        public string OrdersLogicappUrl { get; set; }
    }
}
