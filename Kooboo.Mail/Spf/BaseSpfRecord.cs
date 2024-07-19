using System.Collections.Generic;
using Kooboo.MailTransferAgent.Domain.Validator.Spf;

namespace Kooboo.Mail.Spf
{
    /// <summary>
    /// BaseSpfRecord
    /// class of SPF or SenderID record
    /// </summary>
    public abstract class BaseSpfRecord
    {
        protected BaseSpfRecord(List<TermEvaluation> terms)
        {
            Terms = terms;
        }

        public List<TermEvaluation> Terms { get; set; }

        protected static bool BaseTryParse(string[] terms, out List<TermEvaluation> value)
        {
            value = new List<TermEvaluation>(terms.Length - 1);
            for (int i = 1; i < terms.Length; i++)
            {
                TermEvaluation term;
                if (TermEvaluation.TryParse(terms[i], out term))
                {
                    value.Add(term!);
                }
                else
                {
                    value = null;
                    return false;
                }
            }

            return true;
        }
    }
}

