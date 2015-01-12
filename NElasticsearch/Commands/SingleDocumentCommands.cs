using System.Threading.Tasks;
using NElasticsearch.Models;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/docs.html
    /// </summary>
    public static class SingleDocumentCommands
    {
        public static async Task<GetResponse<T>> Get<T>(this ElasticsearchClient client,
            string id, string typeName, string indexName = null) where T : new()
        {
            var url = (indexName ?? client.DefaultIndexName) + "/" + typeName + "/" + id;            
            return await client.Execute<GetResponse<T>>(RestMethod.GET, url);
        }

        // TODO remove requirement for ID, without type as well
        public static async Task Index<T>(this ElasticsearchClient client,
            T obj, string id, string typeName, string indexName = null)
        {
            var url = (indexName ?? client.DefaultIndexName) + "/" + typeName + (!string.IsNullOrWhiteSpace(id) ? "/" + id : string.Empty);
            await client.Execute(string.IsNullOrWhiteSpace(id) ? RestMethod.POST : RestMethod.PUT, url, obj);
        }

        public static async Task Delete(this ElasticsearchClient client,
            string id, string typeName, string indexName = null)
        {
            var url = (indexName ?? client.DefaultIndexName) + "/" + typeName + "/" + id;
            await client.Execute(RestMethod.DELETE, url);
        }

        // TODO Update API
    }
}
