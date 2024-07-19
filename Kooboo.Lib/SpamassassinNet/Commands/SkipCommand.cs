using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class SkipCommand : CommandBase<ResultBase>
{
    public SkipCommand(string body)
    {
        Body = body;
    }

    protected override string Name => "SKIP";
    protected override string Body { get; }
}