using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class SymbolsCommand : CommandBase<SpamScoreResult>
{
    public SymbolsCommand(string body)
    {
        Body = body;
    }

    protected override string Name => "SYMBOLS";
    protected override string Body { get; }
}