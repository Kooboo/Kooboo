using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class ReportCommand : CommandBase<SpamScoreResult>
{
    public ReportCommand(string body)
    {
        Body = body;
    }

    protected override string Name => "REPORT";
    protected override string Body { get; }
}