using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NElasticsearch.Helpers
{
    static class StringExtensions
    {
        internal static string GetStringValue(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var info = type.GetField(enumValue.ToString());
            var da = (EnumMemberAttribute[])(info.GetCustomAttributes(typeof(EnumMemberAttribute), false));

            if (da.Length > 0)
                return da[0].Value;
            else
                return string.Empty;
        }

        internal static string GetStringValue(this IEnumerable<Enum> enumValues)
        {
            return string.Join(",", enumValues.Select(e => e.GetStringValue()));
        }
        internal static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
