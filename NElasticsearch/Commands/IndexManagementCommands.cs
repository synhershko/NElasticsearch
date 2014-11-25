using System.Net;
using System.Threading.Tasks;
using RestSharp;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html
    /// </summary>
    public static class IndexManagementCommands
    {
        public static void CreateIndex(this ElasticsearchRestClient client, string indexName)
        {
            var request = new RestRequest(indexName, Method.PUT);
            var response = client.Execute(request);
        }

        public static void DeleteIndex(this ElasticsearchRestClient client, string indexName)
        {
            var request = new RestRequest(indexName, Method.DELETE);
            var response = client.Execute(request);
        }

        public static async Task<bool> IndexExists(this ElasticsearchRestClient client, string indexName)
        {
            var request = new RestRequest(indexName, Method.HEAD);
            var response = await client.Execute(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        // TODO Open/Close index API

        public static void Refresh(this ElasticsearchRestClient client, string indexName = null)
        {
            var request = new RestRequest((indexName ?? client.DefaultIndexName) + "/_refresh", Method.POST);
            var response = client.Execute(request);
        }

        // TODO http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html#status-management
        // TODO Clear cache API
        // TODO Flush API
        // TODO Optimize API

        // TODO set replicas
    }
}
