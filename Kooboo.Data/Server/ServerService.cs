using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Server
{
   public static class ServerService
    {

        public static string GetEncodedLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
                return location;
            var builder = new StringBuilder();
            Uri uri;
            // /admin/sites will be parse to uri.schema= file in linux
            if (Uri.TryCreate(location, UriKind.Absolute, out uri))
            {
                if (!string.IsNullOrEmpty(uri.Scheme) &&
                    (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                    uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)))
                {
                    var baseUrl = uri.Scheme + "://" + uri.Authority;
                    builder.Append(baseUrl);
                    location = location.Replace(baseUrl, "");
                }
            }

            var queryString = string.Empty;

            int questionmark = location.IndexOf("?");
            if (questionmark > -1)
            {
                queryString = location.Substring(questionmark);
                location = location.Substring(0, questionmark);

            }
            var segments = location.Split('/');

            for (var i = 0; i < segments.Length; i++)
            {
                var seg = segments[i];
                builder.Append(System.Net.WebUtility.UrlEncode(seg));
                if (segments.Length - 1 != i)
                {
                    builder.Append("/");
                }

            }
            if (!string.IsNullOrEmpty(queryString))
            {
                builder.Append(queryString);
            }
            return builder.ToString();
        }

    }
}
