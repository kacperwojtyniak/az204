using az204api.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace az204api
{
    public static class Extensions
    {
        public static Task<ItemResponse<T>> CreateItemWithIdAsync<T>(this Container container, T item, PartitionKey? partitionKey = null, ItemRequestOptions requestOptions = null, CancellationToken cancellationToken = default) where T : Document
        {
            item.Id = Guid.NewGuid().ToString();
            return container.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);
        }
    }
}
