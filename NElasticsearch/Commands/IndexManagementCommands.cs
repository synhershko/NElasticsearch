using RestSharp;

namespace NElasticsearch.Commands
{
    public static class IndexManagementCommands
    {
        public static void DeleteIndex(this ElasticsearchRestClient client, string indexName)
        {
            var request = new RestRequest(indexName, Method.DELETE);
            var response = client.Execute(request);
        }

        public static void Refresh(this ElasticsearchRestClient client, string indexName = null)
        {
            var request = new RestRequest((indexName ?? client.DefaultIndexName) + "/_refresh", Method.POST);
            var response = client.Execute(request);
        }
    }
}
