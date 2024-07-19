using System.Text.RegularExpressions;
using Kooboo.Mail.Helper;
using Kooboo.Mail.Spf;

namespace Kooboo.MailTransferAgent.Domain.Validator.Spf
{
    /// <summary>
    /// Term Evaluation
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-4.6.1"/>
    /// </summary>
    public abstract class TermEvaluation
    {
        private static readonly Regex _mechanismRegex = new Regex(@"^(\s)*(?<qualifier>[~+?-]?)(?<type>[a-z0-9]+)(:(?<domain>[^/]+))?(/(?<prefix>[0-9]+)(/(?<prefix6>[0-9]+))?)?(\s)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _modifierRegex = new Regex(@"^(\s)*(?<type>[a-z]+)=(?<domain>[^\s]+)(\s)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryParse(string input, out TermEvaluation value)
        {
            if (string.IsNullOrEmpty(input))
            {
                value = null;
                return false;
            }

            var mechanismMatch = _mechanismRegex.Match(input);
            if (mechanismMatch.Success)
            {
                ResultsOfEvaluation resultsOfEvaluation;
                switch (mechanismMatch.Groups["qualifier"].Value)
                {
                    case "+":
                        resultsOfEvaluation = ResultsOfEvaluation.Pass;
                        break;
                    case "-":
                        resultsOfEvaluation = ResultsOfEvaluation.Fail;
                        break;
                    case "~":
                        resultsOfEvaluation = ResultsOfEvaluation.Softfail;
                        break;
                    case "?":
                        resultsOfEvaluation = ResultsOfEvaluation.Neutral;
                        break;
                    default:
                        resultsOfEvaluation = ResultsOfEvaluation.Pass;
                        break;
                }

                MechanismDefinitions mechanism = EnumHelper<MechanismDefinitions>.TryParse(
                    mechanismMatch.Groups["type"].Value, true, out MechanismDefinitions mechanismDefinitions) ? mechanismDefinitions : MechanismDefinitions.Unknown;

                string domain = mechanismMatch.Groups["domain"].Value;
                string tmpPrefix = mechanismMatch.Groups["prefix"].Value;
                int? prefix = null;
                if (!string.IsNullOrEmpty(tmpPrefix) && int.TryParse(tmpPrefix, out int p))
                {
                    prefix = p;
                }

                int? prefix6 = null;
                if (!string.IsNullOrEmpty(tmpPrefix) && int.TryParse(tmpPrefix, out int p6))
                {
                    prefix6 = p6;
                }

                value = new Mechanism(resultsOfEvaluation, mechanism, domain, prefix, prefix6);
                return true;
            }

            var modifierMatch = _modifierRegex.Match(input);
            if (modifierMatch.Success)
            {
                value = new Modifier(
                    EnumHelper<ModifierDefinitions>.TryParse(modifierMatch.Groups["type"].Value, true, out ModifierDefinitions t) ? t : ModifierDefinitions.Unknown,
                    modifierMatch.Groups["domain"].Value);
                return true;
            }

            value = null;
            return false;
        }
    }
}

