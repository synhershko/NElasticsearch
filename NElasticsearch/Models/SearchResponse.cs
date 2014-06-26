using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace NElasticsearch.Models
{
    /// <summary>
    /// Top-level response from ElasticSearch.
    /// </summary>
    [DebuggerDisplay("{hits.hits.Count} hits in {took} ms")]
    public class SearchResponse<T>
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public ShardStatistics _shards { get; set; }
        public Hits<T> hits { get; set; }

        public HttpStatusCode status { get; set; } // TODO always null

        public Dictionary<string, AggregationResultHolder> aggregations { get; set; }
    }

    public class AggregationResultHolder
    {
        public class Bucket
        {
            public string Key { get; set; }
            public int DocCount { get; set; }
        }

        public List<Bucket> Buckets { get; set; }
    }
}
