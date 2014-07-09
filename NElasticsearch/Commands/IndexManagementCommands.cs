using RestSharp;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html
    /// </summary>
    public static class IndexManagementCommands
    {
        // TODO Create index API

        public static void DeleteIndex(this ElasticsearchRestClient client, string indexName)
        {
            var request = new RestRequest(indexName, Method.DELETE);
            var response = client.Execute(request);
        }

        // TODO Exists API
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
