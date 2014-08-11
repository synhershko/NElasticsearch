using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NElasticsearch.Models;
using RestSharp;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/search.html
    /// </summary>
    public static class SearchCommand
    {
        public static SearchResponse<T> Search<T>(this ElasticsearchRestClient client,
            object query, string indexName = null, string typeName = null) where T : new()
        {
            return Search<T>(client, query, (indexName == null) ? null : new[] { indexName }, (typeName == null) ? null : new[] { typeName });
        }

        public static SearchResponse<T> Search<T>(this ElasticsearchRestClient client,
            object query, string[] indexNames = null, string[] typeNames = null) where T : new()
        {
            var response = client.Execute<SearchResponse<T>>(GetSearchRequest(query, indexNames, typeNames));
            VerifySearchResponse(response);
            return response.Data;
        }

        public static async Task<SearchResponse<T>> SearchAsync<T>
            (this ElasticsearchRestClient client,
             object query, string[] indexNames = null,
             string[] typeNames = null) where T : new()
        {
            var searchRequest = GetSearchRequest(query, indexNames, typeNames);
            var taskSource = new TaskCompletionSource<IRestResponse<SearchResponse<T>>>();
            var requestHandle =
                client.ExecuteAsync<SearchResponse<T>>
                    (searchRequest,
                     (restResponse, handle) => taskSource.SetResult(restResponse));

            var response = await taskSource.Task;

            VerifySearchResponse(response);

            return response.Data;
        }

        public static string Search(this ElasticsearchRestClient client,
            object query, string indexName = null, string typeName = null)
        {
            return Search(client, query, (indexName == null) ? null : new[] {indexName}, (typeName == null) ? null : new[] {typeName});
        }

        public static string Search(this ElasticsearchRestClient client,
            object query, string[] indexNames = null, string[] typeNames = null)
        {
            var response = client.Execute(GetSearchRequest(query, indexNames, typeNames));
            VerifySearchResponse(response);
            return response.Content;
        }

        internal static void VerifySearchResponse(IRestResponse response)
        {
            if (response.ErrorException != null || response.StatusCode == 0)
                throw new Exception("NElasticsearch internal error", response.ErrorException);

            // Expected HTTP status code for searches is 200, if we haven't got that something went wrong on the server
            // WebExceptions (e.g. network error or server is unavailable are propogated and handled by the caller
            if (response.StatusCode != HttpStatusCode.OK)
                throw ElasticsearchException.CreateFromResponseBody(response.Content);
        }

        private static RestRequest GetSearchRequest(object query, string[] indexNames = null, string[] typeNames = null)
        {
            var sb = new StringBuilder();
            if (indexNames != null)
            {
                for (var i = 0; i < indexNames.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(indexNames[i]);
                }
                sb.Append('/');
            }
            if (typeNames != null)
            {
                for (var i = 0; i < typeNames.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(typeNames[i]);
                }
                sb.Append('/');
            }
            sb.Append("_search");

            if (query == null)
                return new RestRequest(sb.ToString(), Method.GET);

            var request = new RestRequest(sb.ToString(), Method.POST)
            {
                RequestFormat = DataFormat.Json,
            };
            request.AddBody(query);
            return request;
        }

        // TODO routing support http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/search.html#search-routing
    }
}
