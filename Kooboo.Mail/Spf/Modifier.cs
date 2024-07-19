using System.Text;
using Kooboo.Mail.Helper;
using Kooboo.MailTransferAgent.Domain.Validator.Spf;

namespace Kooboo.Mail.Spf
{
    public class Modifier : TermEvaluation
    {
        public Modifier(ModifierDefinitions modifierType, string domain)
        {
            ModifierType = modifierType;
            Domain = domain;
        }

        public ModifierDefinitions ModifierType { get; set; }

        public string Domain { get; set; } = null!;

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(EnumHelper<ModifierDefinitions>.ToString(ModifierType).ToLower());
            stringBuilder.Append("=");
            stringBuilder.Append(Domain);
            return stringBuilder.ToString();
        }
    }
}
