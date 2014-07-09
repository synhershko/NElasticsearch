using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html#mapping-management
    /// </summary>
    public static class MappingManagementCommands
    {
        public static void PutMapping(this ElasticsearchRestClient client,
            string indexName, string typeName, object mapping)
        {
            PutMapping(client, new[] {indexName}, typeName, mapping);
        }

        public static void PutMapping(this ElasticsearchRestClient client,
            string[] indexNames, string typeName, object mapping)
        {
            var sb = new StringBuilder();
            if (indexNames == null || indexNames.Length == 0)
            {
                sb.Append("*");
            }
            else
            {
                sb.Append(string.Join(",", indexNames));
            }
            sb.Append('/');
            sb.Append(typeName);
            sb.Append("/_mapping");
            var request = new RestRequest(sb.ToString(), Method.PUT);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(mapping);
            var response = client.Execute(request);

            // TODO
            if (response.ErrorException != null)
                throw response.ErrorException;
        }

        // TODO Get mapping API
        // TODO Get Field Mapping API

        public static void DeleteMapping(this ElasticsearchRestClient client,
            string indexName, string typeName)
        {
            var request =
                new RestRequest(indexName + "/" + typeName + "/_mapping", Method.DELETE);

            var response = client.Execute(request);

            // TODO
            if (response.ErrorException != null)
                throw response.ErrorException;
        }

        // TODO type exists API
    }
}
