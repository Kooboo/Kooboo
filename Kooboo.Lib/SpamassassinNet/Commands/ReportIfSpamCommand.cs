using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class ReportIfSpamCommand : CommandBase<ReportIfSpamResult>
{
    public ReportIfSpamCommand(string body)
    {
        Body = body;
    }

    protected override string Name => "REPORT_IFSPAM";
    protected override string Body { get; }
}