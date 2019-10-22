using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DNS.Protocol.Utils {
    public class ObjectStringifier {
        public static ObjectStringifier New(object obj) {
            return new ObjectStringifier(obj);
        }

        public static string Stringify(object obj) {
            return StringifyObject(obj);
        }

        private static string StringifyObject(object obj)
        {
            switch (obj)
            {
                case string s:
                    return s;
                case IDictionary dictionary:
                    return StringifyDictionary(dictionary);
                case IEnumerable enumerable:
                    return StringifyList(enumerable);
                default:
                    return obj == null ? "null" : obj.ToString();
            }
        }

        private static string StringifyList(IEnumerable enumerable) {
            return "[" + string.Join(", ", enumerable.Cast<object>().Select(o => StringifyObject(o)).ToArray()) + "]";
        }

        private static string StringifyDictionary(IDictionary dict) {
            StringBuilder result = new StringBuilder();

            result.Append("{");

            foreach (DictionaryEntry pair in dict) {
                result
                    .Append(pair.Key)
                    .Append("=")
                    .Append(StringifyObject(pair.Value))
                    .Append(", ");
            }

            if (result.Length > 1) {
                result.Remove(result.Length - 2, 2);
            }

            return result.Append("}").ToString();
        }

        private object _obj;
        private Dictionary<string, string> _pairs;

        public ObjectStringifier(object obj) {
            this._obj = obj;
            this._pairs = new Dictionary<string, string>();
        }

        public ObjectStringifier Remove(params string[] names) {
            foreach (string name in names) {
                _pairs.Remove(name);
            }

            return this;
        }

        public ObjectStringifier Add(params string[] names) {
            Type type = _obj.GetType();

            foreach (string name in names) {
                PropertyInfo property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                object value = property?.GetValue(_obj, new object[] { });

                _pairs.Add(name, StringifyObject(value));
            }

            return this;
        }

        public ObjectStringifier Add(string name, object value) {
            _pairs.Add(name, StringifyObject(value));
            return this;
        }

        public ObjectStringifier AddAll() {
            PropertyInfo[] properties = _obj.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties) {
                object value = property.GetValue(_obj, new object[] { });
                _pairs.Add(property.Name, StringifyObject(value));
            }

            return this;
        }

        public override string ToString() {
            return StringifyDictionary(_pairs);
        }
    }
}
