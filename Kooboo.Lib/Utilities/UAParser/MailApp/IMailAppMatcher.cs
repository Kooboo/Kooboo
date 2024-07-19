using System.Collections.Generic;
namespace Kooboo.Lib.Utilities.UAParser.MailApp
{
    public interface IMailAppMatcher
    {
        string MailAppName { get; }
        List<string> SpecialIdentification { get; }

        public bool TryMatch(string userAgentString, out ApplicationInfo application);
    }
}

