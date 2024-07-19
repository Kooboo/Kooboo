using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.MailTransferAgent.Domain.Validator.Spf;

namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// SpfRecord
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-3"/>
    /// </summary>
    public class SpfRecord : BaseSpfRecord
    {
        public SpfRecord(List<TermEvaluation> terms)
            : base(terms)
        {
        }

        public static bool IsSpfRecord(string input)
        {
            return !string.IsNullOrEmpty(input) && input.StartsWith("v=spf1 ", StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryParse(string input, out SpfRecord? value)
        {
            if (!IsSpfRecord(input))
            {
                value = null;
                return false;
            }

            string[] terms = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<TermEvaluation>? result;
            if (BaseTryParse(terms, out result))
            {
                value = new SpfRecord(result!);
                return true;
            }

            value = null;
            return false;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("v=spf1");
            if (Terms!.Any())
            {
                foreach (TermEvaluation term in Terms!)
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
    }
}

