using System.Threading.Tasks;

namespace NElasticsearch
{
    public abstract class ElasticsearchClient
    {
        public string DefaultIndexName { get; set; }

        public abstract Task<T> Execute<T>(RestMethod method, string url, object body = null) where T : new();

        public abstract Task<string> Execute(RestMethod method, string url, object body = null);
    }
}
