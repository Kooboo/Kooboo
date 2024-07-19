using System.Text;
using Kooboo.Mail.Helper;
using Kooboo.MailTransferAgent.Domain.Validator.Spf;

namespace Kooboo.Mail.Spf
{
    public class Mechanism : TermEvaluation
    {
        public Mechanism(ResultsOfEvaluation evaluationType, MechanismDefinitions mechanismType, string domain = null, int? prefix = null, int? prefix6 = null)
        {
            EvaluationType = evaluationType;
            MechanismType = mechanismType;
            Domain = domain;
            Prefix = prefix;
            Prefix6 = prefix6;
        }

        public ResultsOfEvaluation EvaluationType { get; }

        public MechanismDefinitions MechanismType { get; }

        public string Domain { get; }

        public int? Prefix { get; }

        public int? Prefix6 { get; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder
            {
                Capacity = 0,
                Length = 0
            };

            switch (EvaluationType)
            {
                case ResultsOfEvaluation.Fail:
                    stringBuilder.Append("-");
                    break;
                case ResultsOfEvaluation.Softfail:
                    stringBuilder.Append("~");
                    break;
                case ResultsOfEvaluation.Neutral:
                    stringBuilder.Append("?");
                    break;
            }

            stringBuilder.Append(EnumHelper<MechanismDefinitions>.ToString(MechanismType).ToLower());

            if (!string.IsNullOrEmpty(Domain))
            {
                stringBuilder.Append(":");
                stringBuilder.Append(Domain);
            }

            if (Prefix.HasValue)
            {
                stringBuilder.Append("/");
                stringBuilder.Append(Prefix.Value);
            }

            if (Prefix6.HasValue)
            {
                stringBuilder.Append("//");
                stringBuilder.Append(Prefix6.Value);
            }

            return stringBuilder.ToString();
        }
    }
}

