using PayU.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PayU.PaymentHelper
{
    //Bu class PayU dökümanlarından alınmıştır.
    internal class ParameterHandler
    {
        private class ValueHolder
        {
            public string Value { get; set; }
            public bool ExcludeFromHash { get; set; }
            public int SortIndex { get; set; }
        }

        private ILookup<string, ValueHolder> Parameters { get; set; }

        public ParameterHandler(object o)
            : this(o, true)
        {
        }

        public ParameterHandler(object o, bool sort)
        {
            var parameters = ParametersFromType(o, "", false);

            if (sort)
            {
                parameters = parameters.OrderBy(pair => pair.Key);
            }
            else
            {
                parameters = parameters.OrderBy(pair => pair.Value.SortIndex);
            }

            Parameters = parameters.ToLookup(pair => pair.Key, pair => pair.Value);
        }

        private IEnumerable<KeyValuePair<string, ValueHolder>> ParametersFromType(object o, string suffix, bool excludeFromHash)
        {

            var itemType = o.GetType();

            if (itemType.IsGenericType &&
                itemType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return ParametersFromListType(o as IList, excludeFromHash);
            }

            return itemType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .SelectMany(property =>
                            ParametersFromType(o, property, suffix, excludeFromHash));
        }

        private IEnumerable<KeyValuePair<string, ValueHolder>> ParametersFromListType(IList list, bool excludeFromHash)
        {
            var array = new IEnumerable<KeyValuePair<string, ValueHolder>>[list.Count];

            for (var idx = 0; idx < array.Length; idx++)
            {
                array[idx] = ParametersFromType(list[idx], string.Format("[{0}]", idx), excludeFromHash);
            }
            return array.SelectMany(item => item);
        }

        private IEnumerable<KeyValuePair<string, ValueHolder>> ParametersFromType(object o, PropertyInfo property, string suffix, bool excludeFromHash)
        {
            var attributes = property.GetCustomAttributes(false)
                .Where(attr => attr is ParameterAttribute)
                .Cast<ParameterAttribute>()
                .ToList();

            if (!attributes.Any())
                return Enumerable.Empty<KeyValuePair<string, ValueHolder>>();
            var attribute = attributes.First();

            var value = property.GetValue(o, new object[] { });

            if (value == null)
                return Enumerable.Empty<KeyValuePair<string, ValueHolder>>();

            return attribute.IsNested ? ParametersFromType(value, "", attribute.ExcludeFromHash) : new[] { ParameterFromValue(suffix, attribute, value, excludeFromHash) };
        }

        private static KeyValuePair<string, ValueHolder> ParameterFromValue(string suffix, ParameterAttribute attribute, object value, bool excludeFromHash)
        {
            return new KeyValuePair<string, ValueHolder>(
                attribute.Name + suffix,
                new ValueHolder
                {
                    Value = GetStringValue(attribute, value),
                    ExcludeFromHash = excludeFromHash || attribute.ExcludeFromHash,
                    SortIndex = attribute.SortIndex
                }
                );
        }

        private static string GetStringValue(ParameterAttribute attribute, object value)
        {
            if (value is DateTime)
            {
                value = ((DateTime)value).ToUniversalTime();
            }
            else if (value is Boolean)
            {
                value = ((Boolean)value).GetHashCode();
            }
            return string.Format(CultureInfo.InvariantCulture, attribute.FormatString ?? "{0}", value);
        }
        public NameValueCollection GetRequestData()
        {
            var data = new NameValueCollection();
            foreach (var parameter in Parameters)
            {
                foreach (var parameterValue in parameter)
                {
                    data.Add(parameter.Key, parameterValue.Value);
                }
            }
            return data;
        }
        public string GetHashString()
        {
            var str = new StringBuilder();
            foreach (var parameterValue in Parameters
                .SelectMany(parameter => parameter)
                .Where(parameterValue => !parameterValue.ExcludeFromHash))
            {
                str.Append(Encoding.UTF8.GetByteCount(parameterValue.Value));
                str.Append(parameterValue.Value);
            }
            var result = str.ToString();
            return result;
        }
        public string CreateOrderRequestHash(string signatureKey)
        {
            var hashString = GetHashString();
            var hash = hashString.HashWithSignature(signatureKey);
            Parameters["ORDER_HASH"].First().Value = hash;
            return hash;
        }

    }
}