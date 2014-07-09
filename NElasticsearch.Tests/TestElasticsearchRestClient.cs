using System;
using Xunit;

namespace NElasticsearch.Tests
{
    public class TestElasticsearchRestClient
    {
        [Fact]
        public void Invalid_urls_should_throw()
        {
            Assert.Throws<UriFormatException>(() => new ElasticsearchRestClient("dfsdfsd"));
            Assert.Throws<UriFormatException>(() => new ElasticsearchRestClient("10.0.0.5:9200"));
            Assert.DoesNotThrow(() => new ElasticsearchRestClient("http://10.0.0.5"));
            Assert.DoesNotThrow(() => new ElasticsearchRestClient("http://10.0.0.5/"));
            Assert.DoesNotThrow(() => new ElasticsearchRestClient("http://10.0.0.5:9200"));
        }
    }
}
