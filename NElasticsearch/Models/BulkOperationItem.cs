using System.Text;
using RestSharp.Serializers;

namespace NElasticsearch.Models
{
    public abstract class BulkOperationItem
    {
        public abstract void WriteToStringBuilder(StringBuilder sb, JsonSerializer serializer);

        public override string ToString()
        {
            var sb = new StringBuilder();
            WriteToStringBuilder(sb, new JsonSerializer());
            return sb.ToString();
        }

        public class Index : BulkOperationItem
        {
            private readonly string _indexName;
            private readonly string _typeName;
            private readonly object _obj;
            private readonly string _id;

            public Index(string indexName, string typeName, object obj, string id = null)
            {
                _indexName = indexName;
                _typeName = typeName;
                _obj = obj;
                _id = id;
            }

            public override void WriteToStringBuilder(StringBuilder sb, JsonSerializer serializer)
            {
                sb.AppendFormat(@"{{ ""index"" : {{ ""_index"" : ""{0}"", ""_type"" : ""{1}""", _indexName, _typeName);
                if (!string.IsNullOrWhiteSpace(_id))
                    sb.AppendFormat(@", ""_id"" : ""{0}""", _id);
                sb.Append(@" }} }}\n");
                var str = _obj as string;
                if (str != null)
                    sb.Append(str);
                else
                    sb.Append(serializer.Serialize(_obj));
                sb.Append('\n');
            }
        }

        public class Create : BulkOperationItem
        {
            private readonly string _indexName;
            private readonly string _typeName;
            private readonly object _obj;
            private readonly string _id;

            public Create(string indexName, string typeName, object obj, string id)
            {
                _indexName = indexName;
                _typeName = typeName;
                _obj = obj;
                _id = id;
            }

            public override void WriteToStringBuilder(StringBuilder sb, JsonSerializer serializer)
            {
                sb.AppendFormat(@"{{ ""create"" : {{ ""_index"" : ""{0}"", ""_type"" : ""{1}""", _indexName, _typeName);
                sb.AppendFormat(@", ""_id"" : ""{0}""", _id);
                sb.Append(@" }} }}\n");
                var str = _obj as string;
                if (str != null)
                    sb.Append(str);
                else
                    sb.Append(serializer.Serialize(_obj));
                sb.Append('\n');
            }
        }

        public class Delete : BulkOperationItem
        {
            private readonly string _indexName;
            private readonly string _typeName;
            private readonly string _id;

            public Delete(string indexName, string typeName, string id)
            {
                _indexName = indexName;
                _typeName = typeName;
                _id = id;
            }

            public override void WriteToStringBuilder(StringBuilder sb, JsonSerializer serializer)
            {
                sb.AppendFormat(@"{{ ""index"" : {{ ""_index"" : ""{0}"", ""_type"" : ""{1}"", ""_id"" : ""{2}""", _indexName, _typeName, _id);
                sb.Append(@" }} }}\n");
            }
        }
    }
}
