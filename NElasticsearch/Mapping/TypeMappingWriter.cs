using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using NElasticsearch.Helpers;
using RestSharp.Extensions;

namespace NElasticsearch.Mapping
{
    public static class TypeMappingWriter
    {
        // TODO reflection cache

        public static string GetMappingTypeNameFor<T>()
        {
            string typeName;
            var typeAtt = (ElasticsearchTypeAttribute)typeof(T).GetCustomAttribute(typeof(ElasticsearchTypeAttribute));
            if (typeAtt != null)
            {
                typeName = typeAtt.Name ?? typeof(T).Name.ToLowerInvariant();
            }
            else
            {
                typeName = typeof(T).Name.ToLowerInvariant();
            }
            return typeName;
        }

        public static bool GetMappingFor<T>(StringBuilder sb, string typeName = null)
        {
            if (typeName == null)
                typeName = GetMappingTypeNameFor<T>();

            sb.WriteStartObject();
            sb.WritePropertyName(typeName);
            sb.WriteStartObject();
            sb.WritePropertyName("properties");
            var wroteProperties = WritePropertyMappingsFor<T>(sb);
            sb.WriteEndObject(false);
            sb.WriteEndObject(false);

            return wroteProperties;
        }

        public static bool WritePropertyMappingsFor<T>(StringBuilder jsonWriter)
        {
            bool wroteAtLeastOneProperty = false;

            jsonWriter.WriteStartObject();
            var properties = typeof (T).GetProperties();
            foreach (var p in properties)
            {
                var att = (ElasticsearchPropertyAttribute) p.GetCustomAttribute(typeof (ElasticsearchPropertyAttribute));

                // We only write mappings for properties explicitly asking for it, otherwise we rely on Elasticsearch for the defaults
                if (att == null || att.OptOut)
                    continue;

                // TODO consider unchanged attribute as no-op

                var propertyName = att.Name ?? p.Name.ToCamelCase(CultureInfo.InvariantCulture);
                
                var type = GetElasticSearchType(att, p);
                if (type == null) //could not get type from attribute or infer from CLR type.
                    continue;

                if (wroteAtLeastOneProperty && jsonWriter[jsonWriter.Length - 1] != ',') jsonWriter.Append(',');
                wroteAtLeastOneProperty = true;
                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartObject();
                {
                    WritePropertiesFromAttribute(jsonWriter, att, propertyName, type);

                    // TODO nested objects are not supported at this point
//                    if (type == "object" || type == "nested")
//                    {
//
//                        var deepType = GetUnderlyingType(p.PropertyType);
//                        var deepTypeName = this.Infer.TypeName(deepType);
//                        var seenTypes = new ConcurrentDictionary<Type, int>(this.SeenTypes);
//                        seenTypes.AddOrUpdate(deepType, 0, (t, i) => ++i);
//
//                        var newTypeMappingWriter = new TypeMappingWriter(deepType, deepTypeName, this._connectionSettings, MaxRecursion, seenTypes);
//                        var nestedProperties = newTypeMappingWriter.MapPropertiesFromAttributes();
//
//                        jsonWriter.WritePropertyName("properties");
//                        nestedProperties.WriteTo(jsonWriter);
//                    }
                }
                jsonWriter.WriteEndObject();
            }
            jsonWriter.WriteEndObject(false);

            return wroteAtLeastOneProperty;
        }

        static void WritePropertiesFromAttribute(this StringBuilder _jsonWriter, ElasticsearchPropertyAttribute att, string _propertyName, string _type)
        {
            if (att.AddSortField)
            {
                _jsonWriter.WritePropertyName("type");
                _jsonWriter.WriteValue("multi_field");
                _jsonWriter.WritePropertyName("fields");
                _jsonWriter.WriteStartObject();
                _jsonWriter.WritePropertyName(_propertyName);
                _jsonWriter.WriteStartObject();
            }
            if (att.NumericType != NumberType.Default)
            {
                _jsonWriter.WritePropertyName("type");
                var numericType = att.NumericType.GetStringValue();
                _jsonWriter.WriteValue(numericType.ToLowerInvariant());
            }
            else
            {
                _jsonWriter.WritePropertyName("type");
                _jsonWriter.WriteValue(_type);
            }
            if (!att.Analyzer.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("analyzer");
                _jsonWriter.WriteValue(att.Analyzer);
            }
            if (!att.IndexAnalyzer.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("index_analyzer");
                _jsonWriter.WriteValue(att.IndexAnalyzer);
            }
            if (!att.IndexAnalyzer.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("index_analyzer");
                _jsonWriter.WriteValue(att.IndexAnalyzer);
            }
            if (!att.NullValue.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("null_value");
                _jsonWriter.WriteValue(att.NullValue);
            }
            if (!att.SearchAnalyzer.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("search_analyzer");
                _jsonWriter.WriteValue(att.SearchAnalyzer);
            }
            if (!att.DateFormat.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("format");
                _jsonWriter.WriteValue(att.DateFormat);
            }
            if (att.Index != FieldIndexOption.Analyzed)
            {
                _jsonWriter.WritePropertyName("index");
                _jsonWriter.WriteValue(att.Index.GetStringValue());
            }
            if (att.TermVector != TermVectorOption.No)
            {
                _jsonWriter.WritePropertyName("term_vector");
                _jsonWriter.WriteValue(att.TermVector.GetStringValue());
            }
            if (att.OmitNorms)
            {
                _jsonWriter.WritePropertyName("omit_norms");
                _jsonWriter.WriteValue("true");
            }
            if (att.DocValues)
            {
                _jsonWriter.WritePropertyName("doc_values");
                _jsonWriter.WriteValue("true");
            }
            if (att.OmitTermFrequencyAndPositions)
            {
                _jsonWriter.WritePropertyName("omit_term_freq_and_positions");
                _jsonWriter.WriteValue("true");
            }
            if (!att.IncludeInAll)
            {
                _jsonWriter.WritePropertyName("include_in_all");
                _jsonWriter.WriteValue("false");
            }
            if (att.Store)
            {
                _jsonWriter.WritePropertyName("store");
                _jsonWriter.WriteValue("true");
            }
            if (Math.Abs(att.Boost - 1) > Double.Epsilon)
            {
                _jsonWriter.WritePropertyName("boost");
                _jsonWriter.WriteRawValue(att.Boost.ToString(CultureInfo.InvariantCulture));
            }
            if (att.PrecisionStep != 4)
            {
                _jsonWriter.WritePropertyName("precision_step");
                _jsonWriter.WriteRawValue(att.PrecisionStep.ToString(CultureInfo.InvariantCulture));
            }

            if (!att.Similarity.IsNullOrEmpty())
            {
                _jsonWriter.WritePropertyName("similarity");
                _jsonWriter.WriteValue(att.Similarity);
            }
            if (att.AddSortField)
            {
                _jsonWriter.WriteEndObject();
                _jsonWriter.WritePropertyName("sort");
                _jsonWriter.WriteStartObject();

                if (att.NumericType != NumberType.Default)
                {
                    _jsonWriter.WritePropertyName("type");
                    string numericType = att.NumericType.GetStringValue();
                    _jsonWriter.WriteValue(numericType.ToLowerInvariant());
                }
                else
                {
                    _jsonWriter.WritePropertyName("type");
                    _jsonWriter.WriteValue(_type);
                }
                if (att.SortAnalyzer.IsNullOrEmpty())
                {
                    _jsonWriter.WritePropertyName("index");
                    _jsonWriter.WriteValue(FieldIndexOption.NotAnalyzed.GetStringValue());
                }
                else
                {
                    _jsonWriter.WritePropertyName("index_analyzer");
                    _jsonWriter.WriteValue(att.SortAnalyzer);
                }

                _jsonWriter.WriteEndObject();
                _jsonWriter.WriteEndObject();
            }
        }

        static void WritePropertyName(this StringBuilder sb, string name)
        {
            sb.Append('"').Append(name).Append('"').Append(':');
        }

        static void WriteValue(this StringBuilder sb, string value)
        {
            sb.Append('"').Append(value.Replace("\"", "\\\"")).Append('"').Append(',');
        }

        static void WriteRawValue(this StringBuilder sb, string value)
        {
            sb.Append(value).Append(',');
        }

        static void WriteStartObject(this StringBuilder sb)
        {
            sb.Append('{');
        }

        static void WriteEndObject(this StringBuilder sb, bool expectingMore = true)
        {
            if (sb[sb.Length - 1] == ',') sb.Length = sb.Length - 1;
            sb.Append('}');
            if (expectingMore) sb.Append(',');
        }

        /// <summary>
        /// Get the Elastic Search Field Type Related.
        /// </summary>
        /// <param name="att">ElasticPropertyAttribute</param>
        /// <param name="p">Property Field</param>
        /// <returns>String with the type name or null if can not be inferres</returns>
        private static string GetElasticSearchType(ElasticsearchPropertyAttribute att, PropertyInfo p)
        {
            FieldType? fieldType = att.Type;
            if (fieldType == FieldType.None)
            {
                fieldType = GetFieldTypeFromType(p.PropertyType);
            }

            return GetElasticSearchTypeFromFieldType(fieldType);
        }

        /// <summary>
        /// Get the Elastic Search Field from a FieldType.
        /// </summary>
        /// <param name="fieldType">FieldType</param>
        /// <returns>String with the type name or null if can not be inferres</returns>
        private static string GetElasticSearchTypeFromFieldType(FieldType? fieldType)
        {
            switch (fieldType)
            {
                case FieldType.GeoPoint:
                    return "geo_point";
                case FieldType.GeoShape:
                    return "geo_shape";
                case FieldType.Attachment:
                    return "attachment";
                case FieldType.Ip:
                    return "ip";
                case FieldType.Binary:
                    return "binary";
                case FieldType.String:
                    return "string";
                case FieldType.Integer:
                    return "integer";
                case FieldType.Long:
                    return "long";
                case FieldType.Float:
                    return "float";
                case FieldType.Double:
                    return "double";
                case FieldType.Date:
                    return "date";
                case FieldType.Boolean:
                    return "boolean";
                case FieldType.Completion:
                    return "completion";
                case FieldType.Nested:
                    return "nested";
                case FieldType.Object:
                    return "object";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Inferes the FieldType from the type of the property.
        /// </summary>
        /// <param name="propertyType">Type of the property</param>
        /// <returns>FieldType or null if can not be inferred</returns>
        private static FieldType? GetFieldTypeFromType(Type propertyType)
        {
            propertyType = GetUnderlyingType(propertyType);

            if (propertyType == typeof(string))
                return FieldType.String;

            if (propertyType.IsValueType)
            {
                switch (propertyType.Name)
                {
                    case "Int32":
                        return FieldType.Integer;
                    case "Int64":
                        return FieldType.Long;
                    case "Single":
                        return FieldType.Float;
                    case "Decimal":
                    case "Double":
                        return FieldType.Double;
                    case "DateTime":
                        return FieldType.Date;
                    case "Boolean":
                        return FieldType.Boolean;
                }
            }
            else
                return FieldType.Object;
            return null;
        }

        private static Type GetUnderlyingType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.GetGenericArguments().Length >= 1)
                return type.GetGenericArguments()[0];

            return type;
        }
    }
}
