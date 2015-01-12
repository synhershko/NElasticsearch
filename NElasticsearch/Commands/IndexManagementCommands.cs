using System.Threading.Tasks;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html
    /// </summary>
    public static class IndexManagementCommands
    {
        public static async Task CreateIndex(this ElasticsearchClient client, string indexName)
        {
            await client.Execute(RestMethod.PUT, indexName);
        }

        public static async Task DeleteIndex(this ElasticsearchClient client, string indexName)
        {
            await client.Execute(RestMethod.DELETE, indexName);
        }

        public static async Task<bool> IndexExists(this ElasticsearchClient client, string indexName)
        {
            try
            {
                await client.Execute(RestMethod.HEAD, indexName);
                return true;
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
        }

        // TODO Open/Close index API

        public static async Task Refresh(this ElasticsearchClient client, string indexName = null)
        {
            await client.Execute(RestMethod.POST, (indexName ?? client.DefaultIndexName) + "/_refresh");
        }

        // TODO http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html#status-management
        // TODO Clear cache API
        // TODO Flush API
        // TODO Optimize API

        // TODO set replicas
    }
}
