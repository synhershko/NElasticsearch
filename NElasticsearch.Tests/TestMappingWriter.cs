using System.Text;
using NElasticsearch.Mapping;
using NElasticsearch.Tests.TestModels;
using Xunit;

namespace NElasticsearch.Tests
{
    public class TestMappingWriter
    {
        [Fact]
        public void No_mapping_attributes_should_result_in_noop()
        {
            var sb = new StringBuilder();
            Assert.False(TypeMappingWriter.WritePropertyMappingsFor<File>(sb));
            Assert.False(TypeMappingWriter.WritePropertyMappingsFor<FileWithEmptyAtts>(sb));
        }

        [Fact]
        public void Mapping_attributes_generates_correct_json()
        {
            var sb = new StringBuilder();

            Assert.True(TypeMappingWriter.WritePropertyMappingsFor<FileWithOneAtt>(sb));
            Assert.Equal(@"{""file_type"":{""type"":""string""}}", sb.ToString());

            sb.Length = 0;
            Assert.True(TypeMappingWriter.WritePropertyMappingsFor<FileWithTwoAtts>(sb));
            Assert.Equal(@"{""fileType"":{""type"":""string"",""index"":""not_analyzed""},""size"":{""type"":""integer""}}", sb.ToString());
        }

        [Fact]
        public void Get_mapping_generates_correct_json()
        {
            var sb = new StringBuilder();

            Assert.True(TypeMappingWriter.GetMappingFor<FileWithOneAtt>(sb, "test"));
            Assert.Equal(@"{""test"":{""properties"":{""file_type"":{""type"":""string""}}}}", sb.ToString());

            sb.Length = 0;
            Assert.True(TypeMappingWriter.GetMappingFor<FileWithTwoAtts>(sb, "test"));
            Assert.Equal(@"{""test"":{""properties"":{""fileType"":{""type"":""string"",""index"":""not_analyzed""},""size"":{""type"":""integer""}}}}", sb.ToString());
        }

        class FileWithEmptyAtts
        {
            public string FileType { get; set; }

            [ElasticsearchProperty(OptOut = true)]
            public string Path { get; set; }
        }

        class FileWithOneAtt
        {
            [ElasticsearchProperty(Name = "file_type")]
            public string FileType { get; set; }
            public string Path { get; set; }
        }

        class FileWithTwoAtts
        {
            [ElasticsearchProperty(Index = FieldIndexOption.NotAnalyzed)]
            public string FileType { get; set; }
            public string Path { get; set; }
            [ElasticsearchProperty]
            public int Size { get; set; }
        }
    }
}
