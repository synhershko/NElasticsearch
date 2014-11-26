using System.Text;

namespace NElasticsearch.Helpers
{
    internal static class ElasticsearchUrlBuilder
    {
        internal static string BuildUrl(string endpoint, string[] indexNames = null, string[] typeNames = null)
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
            sb.Append(endpoint);
            return sb.ToString();
        }
    }
}
