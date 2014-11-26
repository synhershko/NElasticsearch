namespace NElasticsearch
{
    public class ResourceNotFoundException : ElasticsearchException
    {
        public string Url { get; set; }

        public ResourceNotFoundException(string url) : base("The requested resource wasn't found", 404)
        {
            Url = url;
        }
    }
}
