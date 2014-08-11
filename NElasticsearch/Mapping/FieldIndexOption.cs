using System.Runtime.Serialization;

namespace NElasticsearch.Mapping
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum FieldIndexOption
    {
        [EnumMember(Value = "analyzed")]
        Analyzed,
        [EnumMember(Value = "not_analyzed")]
        NotAnalyzed,
        [EnumMember(Value = "no")]
        No
    }
}
