using System;

namespace NElasticsearch.Mapping
{
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class ElasticsearchTypeAttribute : Attribute
    {
        public string Name { get; set; }
        public string IdProperty { get; set; }
    }
}
