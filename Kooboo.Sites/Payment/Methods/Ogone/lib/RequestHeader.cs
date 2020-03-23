using System;
using System.Text.RegularExpressions;


namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class RequestHeader
    {
        public RequestHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is required");
            }
            Name = name;
            Value = NormalizeValue(value);
        }

        private string NormalizeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            // Replace all sequences of whitespace*-linebreak-whitespace* into a single linebreak-space
            // This will ensure that:
            // - no line ends with whitespace, because this causes authentication failures
            // - each line starts with a single whitespace, so it is a valid header value
            var pattern = "[\\s-[\r\n]]*(\r?\n)[\\s-[\r\n]]*";
            var newString = new Regex(pattern, RegexOptions.Multiline | RegexOptions.CultureInvariant).Replace(value, "$1 ");
            return newString;
        }

        #region IRequestHeader
        public string Name { get; }

        public string Value { get; }
        #endregion

        public override string ToString() => Name + ":" + Value;

        public override int GetHashCode()
            => Tuple.Create(Name, Value).GetHashCode();

        public bool Equals(RequestHeader obj)
            => (obj?.Name?.Equals(Name) ?? false)
                && (obj?.Value?.Equals(Value) ?? false);

        public override bool Equals(object obj)
            => obj is RequestHeader && Equals(obj as RequestHeader);
    }
}
