using NElasticsearch.Commands;
using Xunit;

namespace NElasticsearch.Tests
{
    public class TestSingleDocumentCommands
    {
        [Fact]
        public void Can_add_get_and_delete()
        {
            var client = new ElasticsearchRestClient("http://localhost:9200");
            client.Index(new {foo = "bar"}, "0000", "test", "test");
            Assert.NotNull(client.Get<dynamic>("0000", "test", "test"));
            client.Delete("0000", "test", "test");
        }
    }
}
