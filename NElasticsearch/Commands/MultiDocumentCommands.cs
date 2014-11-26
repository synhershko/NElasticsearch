using System.Collections.Generic;
using System.Text;
using NElasticsearch.Helpers;
using NElasticsearch.Models;
using RestSharp.Serializers;

namespace NElasticsearch.Commands
{
    public static class MultiDocumentCommands
    {
        // TODO Multi-Get API

        public static void Bulk(this ElasticsearchClient client, BulkOperation bulkOperation)
        {
            Bulk(client, bulkOperation.BulkOperationItems);
        }

        public static async void Bulk(this ElasticsearchClient client, IEnumerable<BulkOperationItem> bulkOperationsItem)
        {          
            var sb = new StringBuilder();
            var serializer = new JsonSerializer();
            foreach (var item in bulkOperationsItem)
            {
                item.WriteToStringBuilder(sb, serializer);
            }
            await client.Execute(RestMethod.POST, "/_bulk", sb.ToString());            
        }

        // TODO Bulk UDP API
        // TODO Delete by query API
    }
}
