using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.HttpServer
{
    public class HttpMessage
    {
        public string StartLine { get; set; }

        private Dictionary<string, string> _header;

        public Dictionary<string, string> Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        public byte[] Body { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(StartLine);

            foreach (var each in Header)
            {
                builder.AppendLine($"{each.Key}: {each.Value}");
            }

            builder.AppendLine();

            if (Body != null)
            {
                builder.Append(Encoding.UTF8.GetString(Body));
            }

            return builder.ToString();
        }
    }
}