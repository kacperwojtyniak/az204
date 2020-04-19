using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace az204api.Models
{
	public class QueryResponse<T>
	{
		public T[] Documents { get; set; }
	}
}
