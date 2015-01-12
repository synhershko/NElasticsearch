using System;
using System.Diagnostics;
using NElasticsearch.Commands;
using NElasticsearch.Tests.TestModels;
using Xunit;

namespace NElasticsearch.Tests
{
    public class TestSingleDocumentCommands
    {
        [Fact(Skip = "Ignored for build servers with ES running locally")]
        public async void Can_add_get_and_delete()
        {
            var client = new ElasticsearchRestClient("http://localhost:9200");
            client.Index(new File { FileType = "jpg", Path = @"foo\bar" }, "1111", "test", "test").Wait();
            client.Refresh("test").Wait();

            var getTask = client.Get<File>("1111", "test", "test");
            getTask.Wait();
            Assert.True(getTask.IsCompleted);

            var response = getTask.Result;
            Console.WriteLine("Found document {0}", response._id);
            Assert.Equal("jpg", response._source.FileType);
            Assert.Equal(@"foo\bar", response._source.Path);
            client.Delete("1111", "test", "test").Wait();
        }
    }
}
