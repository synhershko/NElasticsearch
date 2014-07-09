using System.Net;
using NElasticsearch.Models;
using RestSharp;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/docs.html
    /// </summary>
    public static class SingleDocumentCommands
    {
        public static GetResponse<T> Get<T>(this ElasticsearchRestClient client,
            string id, string typeName, string indexName = null) where T : new()
        {
            var request = new RestRequest((indexName ?? client.DefaultIndexName) + "/" + typeName + "/{id}", Method.GET);
            request.AddUrlSegment("id", id);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute<GetResponse<T>>(request);
            if (response.ErrorException != null)
                throw response.ErrorException;

            // TODO post-processing
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            return response.Data;
        }

        // TODO remove requirement for ID, without type as well
        public static void Index<T>(this ElasticsearchRestClient client,
            T obj, string id, string typeName, string indexName = null)
        {
            var request =
                new RestRequest((indexName ?? client.DefaultIndexName) + "/" + typeName +
                                (!string.IsNullOrWhiteSpace(id) ? "/" + id : string.Empty), Method.POST);


            request.RequestFormat = DataFormat.Json;
            request.AddBody(obj);
            var response = client.Execute(request);

            // TODO
            if (response.ErrorException != null)
                throw response.ErrorException;
        }

        public static void Delete(this ElasticsearchRestClient client,
            string id, string typeName, string indexName = null)
        {
            var request = new RestRequest((indexName ?? client.DefaultIndexName) + "/" + typeName + "/{id}", Method.DELETE);
            request.AddUrlSegment("id", id);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            if (response.ErrorException != null)
                throw response.ErrorException;            
        }

        // TODO Update API
    }
}
