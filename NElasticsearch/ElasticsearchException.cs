using System;
using RestSharp;

namespace NElasticsearch
{
    [Serializable]
    public class ElasticsearchException : Exception
    {
        public ElasticsearchException(string message, int statusCode = -1) : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; private set; }

        public static ElasticsearchException CreateFromResponseBody(string body)
        {
            var obj = (JsonObject) SimpleJson.DeserializeObject(body);
            if (obj.ContainsKey("error") && obj.ContainsKey("status"))
                return new ElasticsearchException(obj["error"].ToString(), int.Parse(obj["status"].ToString()));
            return new ElasticsearchException(body);
        }
    }
}
