using System.Collections.Generic;
using System.Net;
using System.Text;
using NElasticsearch.Helpers;
using NElasticsearch.Models;
using RestSharp;
using RestSharp.Serializers;

namespace NElasticsearch.Commands
{
    public static class MultiDocumentCommands
    {
        // TODO Multi-Get API

        public static void Bulk(this ElasticsearchRestClient client, BulkOperation bulkOperation)
        {
            Bulk(client, bulkOperation.BulkOperationItems);
        }

        public static void Bulk(this ElasticsearchRestClient client, IEnumerable<BulkOperationItem> bulkOperationsItem)
        {
            var request = new RestRequest("/_bulk", Method.POST);

            var sb = new StringBuilder();
            var serializer = new JsonSerializer();
            foreach (var item in bulkOperationsItem)
            {
                item.WriteToStringBuilder(sb, serializer);
            }
            request.AddBody(sb.ToString());
            var response = client.Execute(request);

            // TODO
            if (response.ErrorException != null)
                throw response.ErrorException;
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw ElasticsearchException.CreateFromResponseBody(response.Content);
        }

        // TODO Bulk UDP API
        // TODO Delete by query API
    }
}
