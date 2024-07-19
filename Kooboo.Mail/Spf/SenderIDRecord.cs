using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Kooboo.Mail.Helper;
using Kooboo.MailTransferAgent.Domain.Validator.Spf;

namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// SenderIDRecord base BaseSpfRecord
    /// <see href="https://datatracker.ietf.org/doc/html/rfc4406#section-3"/>
    /// </summary>
    public class SenderIDRecord : BaseSpfRecord
    {
        private static readonly Regex _prefixRegex = new Regex(@"^v=spf((?<version>1)|(?<version>2)\.(?<minor>\d)/(?<scopes>(([a-z0-9]+,)*[a-z0-9]+)))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SenderIDRecord(int version, int minorVersion, List<SenderIDScopes> scopes, List<TermEvaluation> terms)
            : base(terms)
        {
            Version = version;
            MinorVersion = minorVersion;
            Scopes = scopes;
        }

        /// <summary>
        /// Version of Sender ID Record
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Minor Version of the Sender ID Record
        /// </summary>
        public int MinorVersion { get; set; }

        public List<SenderIDScopes> Scopes { get; set; }

        public static bool IsSenderIDRecord(string input, SenderIDScopes scope)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            string[] terms = input.Split(new[] { ' ' }, 2);
            if (terms.Length < 2)
                return false;
            int version;
            int minorVersion;
            List<SenderIDScopes>? scopes;
            if (!TryParsePrefix(terms[0], out version, out minorVersion, out scopes))
            {
                return false;
            }

            if (version == 1 && (scope == SenderIDScopes.MFrom || scope == SenderIDScopes.Pra))
            {
                return true;
            }
            else
            {
                return scopes!.Contains(scope);
            }
        }

        public static bool TryParse(string input, out SenderIDRecord? value)
        {
            if (string.IsNullOrEmpty(input))
            {
                value = null;
                return false;
            }

            string[] terms = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (terms.Length < 1)
            {
                value = null;
                return false;
            }

            int version;
            int minorVersion;
            List<SenderIDScopes>? scopes;
            if (!TryParsePrefix(terms[0], out version, out minorVersion, out scopes))
            {
                value = null;
                return false;
            }

            List<TermEvaluation>? evaluation;
            if (BaseTryParse(terms, out evaluation))
            {
                value = new SenderIDRecord(version, minorVersion, scopes!, evaluation!);
                return true;
            }

            value = null;
            return false;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (Version == 1)
            {
                stringBuilder.Append("v=spf1");
            }
            else
            {
                stringBuilder.Append("v=spf");
                stringBuilder.Append(Version);
                stringBuilder.Append(".");
                stringBuilder.Append(MinorVersion);
                stringBuilder.Append("/");
                stringBuilder.Append(string.Join(",", Scopes
                    .Where(x => x != SenderIDScopes.Unknown)
                    .Select(x => EnumHelper<SenderIDScopes>.ToString(x).ToLower())));
            }

            if (Terms!.Any())
            {
                foreach (var term in Terms!)
                {
                    var modifier = term as Modifier;
                    if (modifier == null || modifier.ModifierType != ModifierDefinitions.Unknown)
                    {
                        stringBuilder.Append(" ");
                        stringBuilder.Append(term);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        private static bool TryParsePrefix(string prefix, out int version, out int minorVersion, out List<SenderIDScopes> scopes)
        {
            var prefixMatch = _prefixRegex.Match(prefix);
            if (!prefixMatch.Success)
            {
                version = 0;
                minorVersion = 0;
                scopes = null;
                return false;
            }

            version = int.Parse(prefixMatch.Groups["version"].Value);
            minorVersion = int.Parse(prefixMatch.Groups["minor"].Value);
            scopes = prefixMatch.Groups["scopes"].Value.Split(",").Select(t => EnumHelper<SenderIDScopes>.Parse(t, true, SenderIDScopes.Unknown)).ToList();
            return true;
        }
    }
}

