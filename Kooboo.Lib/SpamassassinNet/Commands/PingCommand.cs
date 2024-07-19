using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class PingCommand : CommandBase<BasicResult>
{
    protected override string Name => "PING";
    protected override string Body => string.Empty;
}