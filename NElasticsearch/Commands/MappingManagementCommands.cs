using System.Reflection;
using System.Text;
using NElasticsearch.Mapping;
using RestSharp;

namespace NElasticsearch.Commands
{
    /// <summary>
    /// See http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/indices.html#mapping-management
    /// </summary>
    public static class MappingManagementCommands
    {
        public static void PutMappingFor<T>(this ElasticsearchRestClient client, string indexName)
        {
            PutMappingFor<T>(client, new[] { indexName });
        }

        public static void PutMappingFor<T>(this ElasticsearchRestClient client, string[] indexNames)
        {
            var typeName = TypeMappingWriter.GetMappingTypeNameFor<T>();

            // We are only going to do a put mapping if there are attributes asking for it
            var sb = new StringBuilder();
            if (TypeMappingWriter.GetMappingFor<T>(sb, typeName))
            {
                PutMapping(client, indexNames, typeName, sb.ToString());
            }
        }
        
        public static void PutMapping(this ElasticsearchRestClient client,
            string indexName, string typeName, object mapping)
        {
            PutMapping(client, new[] {indexName}, typeName, mapping);
        }

        public static async void PutMapping(this ElasticsearchRestClient client,
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

            var mappingString = mapping as string;
            if (mappingString != null)
                request.AddParameter("text/json", mapping, ParameterType.RequestBody);
            else
                request.AddBody(mapping);
            var response = await client.Execute(request);

            // TODO
            if (response.ErrorException != null)
                throw response.ErrorException;
        }

        // TODO Get mapping API
        // TODO Get Field Mapping API

        public static async void DeleteMapping(this ElasticsearchRestClient client,
            string indexName, string typeName)
        {
            var request =
                new RestRequest(indexName + "/" + typeName + "/_mapping", Method.DELETE);

            var response = await client.Execute(request);

            // TODO
            if (response.ErrorException != null)
                throw response.ErrorException;
        }

        // TODO type exists API
    }
}
