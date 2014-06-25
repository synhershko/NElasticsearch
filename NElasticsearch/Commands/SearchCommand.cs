using System;
using System.Net;
using System.Text;
using NElasticsearch.Models;
using RestSharp;

namespace NElasticsearch.Commands
{
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
            if (response.StatusCode != HttpStatusCode.OK)
                throw ElasticsearchException.CreateFromResponseBody(response.Content);
            if (response.ErrorException != null)
                throw new Exception("NElasticsearch internal error", response.ErrorException);
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
    }
}
