using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class ProcessCommand : CommandBase<SpamScoreResult>
{
    public ProcessCommand(string body)
    {
        Body = body;
    }

    protected override string Name => "PROCESS";
    protected override string Body { get; }
}