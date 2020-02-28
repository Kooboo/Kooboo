using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.XMLCommon
{
    public class QueryString
    {
        protected StringBuilder builder;

        public QueryString()
        {
            builder = new StringBuilder();
        }

        public virtual QueryString Append(KeyValuePair<string, string> pair)
        {
            return Append(pair.Key.Replace('-', '_'), pair.Value);
        }

        public virtual QueryString Append(string key, object value)
        {
            if (value == null)
            {
                return this;
            }
            if (value is Request)
            {
                return AppendRequest(key, (Request)value);
            }
            if (value is bool)
            {
                return AppendString(key, value.ToString().ToLower());
            }
            else if (value is Dictionary<string, string>)
            {
                foreach (var pair in (Dictionary<string, string>)value)
                {
                    AppendString(string.Format("{0}[{1}]", key, pair.Key), pair.Value);
                }
                return this;
            }

            return AppendString(key, value.ToString());
        }

        protected virtual QueryString AppendString(string key, string value)
        {
            if (key != null && !(key == "") && value != null)
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }
                builder.Append(EncodeParam(key, value));
            }
            return this;
        }

        protected virtual QueryString AppendRequest(string parent, Request request)
        {
            if (request == null)
            {
                return this;
            }
            string requestQueryString = request.ToQueryString(parent);
            if (requestQueryString.Length > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }

                builder.Append(requestQueryString);
            }
            return this;
        }


        protected virtual string EncodeParam(string key, string value)
        {
            return WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
