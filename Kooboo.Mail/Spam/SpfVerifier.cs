using System.Threading.Tasks;
using Kooboo.Mail.Spf;

namespace Kooboo.Mail.Spam
{
    public static class SpfVerifier
    {
        private static SpfValidator spfValidator = new SpfValidator();
        public static async Task<TestResult> Validate(string MailFrom, System.Net.IPAddress IP)
        {
            var address = Kooboo.Mail.Utility.AddressUtility.GetAddress(MailFrom);

            var SegMent = Utility.AddressUtility.ParseSegment(address);

            if (SegMent.Host != null)
            {
                if (DomainName.TryParse(SegMent.Host, out var domainName))
                {
                    var spfResult = await spfValidator.CheckHost(IP, domainName, address);

                    if (spfResult != null)
                    {
                        if (spfResult.Result == ResultsOfEvaluation.Pass)
                        {
                            return TestResult.Pass;
                        }
                        else if (spfResult.Result == ResultsOfEvaluation.Fail || spfResult.Result == ResultsOfEvaluation.Softfail || spfResult.Result == ResultsOfEvaluation.None)
                        {
                            return TestResult.Failed;
                        }
                    }
                }
            }

            return TestResult.NoResult;

        }

    }
}
