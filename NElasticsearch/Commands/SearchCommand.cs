using System.Threading.Tasks;
using NElasticsearch.Helpers;
using NElasticsearch.Models;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/search.html
    /// </summary>
    public static class SearchCommand
    {
        public static async Task<SearchResponse<T>> Search<T>(this ElasticsearchClient client,
            object query, string indexName = null, string typeName = null) where T : new()
        {
            return await client.Search<T>(query, (indexName == null) ? null : new[] { indexName }, (typeName == null) ? null : new[] { typeName });
        }

        public static async Task<string> Search(this ElasticsearchClient client,
            object query, string indexName = null, string typeName = null)
        {
            return await Search(client, query, (indexName == null) ? null : new[] {indexName}, (typeName == null) ? null : new[] {typeName});
        }

        public static async Task<SearchResponse<T>> Search<T>(this ElasticsearchClient client,
            object query, string[] indexNames = null, string[] typeNames = null) where T : new()
        {
            var url = ElasticsearchUrlBuilder.BuildUrl("_search", indexNames, typeNames);

            if (query == null)
            {
                return await client.Execute<SearchResponse<T>>(RestMethod.GET, url);
            }

            return await client.Execute<SearchResponse<T>>(RestMethod.POST, url, query);
        }

        public static async Task<string> Search(this ElasticsearchClient client,
            object query, string[] indexNames = null, string[] typeNames = null)
        {
            var url = ElasticsearchUrlBuilder.BuildUrl("_search", indexNames, typeNames);

            if (query == null)
            {
                return await client.Execute(RestMethod.GET, url);
            }

            return await client.Execute(RestMethod.POST, url, query);
        }

        // TODO routing support http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/search.html#search-routing
    }
}
