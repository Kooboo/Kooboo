using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class CheckCommand : CommandBase<SpamScoreResult>
{
    public CheckCommand(string body)
    {
        Body = body;
    }

    protected override string Name => "CHECK";
    protected override string Body { get; }
}