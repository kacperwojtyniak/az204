using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace az204api.Models
{
    public class QueryResult
    {
        public string ContinuationToken { get; }
        public IEnumerable<object> Result { get; }
        public string RequestCharge { get; }

        public QueryResult(string continuationToken, IEnumerable<object> result, string requestCharge)
        {            
            ContinuationToken = string.IsNullOrEmpty(continuationToken) ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(continuationToken));
            Result = result;
            RequestCharge = requestCharge;
        }
    }
}
