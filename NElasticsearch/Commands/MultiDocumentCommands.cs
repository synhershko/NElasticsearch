using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NElasticsearch.Helpers;
using NElasticsearch.Models;
using RestSharp.Serializers;

namespace NElasticsearch.Commands
{
    public static class MultiDocumentCommands
    {
        // TODO Multi-Get API

        public static async Task Bulk(this ElasticsearchClient client, BulkOperation bulkOperation)
        {
            await Bulk(client, bulkOperation.BulkOperationItems);
        }

        public static async Task Bulk(this ElasticsearchClient client, IEnumerable<BulkOperationItem> bulkOperationsItem)
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
