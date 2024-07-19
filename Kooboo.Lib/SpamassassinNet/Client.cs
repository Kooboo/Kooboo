using System;
using System.Threading.Tasks;
using Kooboo.ApiMarket;
using SpamassassinNet.Commands;

namespace SpamassassinNet;

public class SpamassassinClient
{
    private readonly ClientOptions _options;

    public SpamassassinClient(ClientOptions options)
    {
        _options = options;
    }

    public async Task<T> SendAsync<T>(CommandBase<T> command) where T : ResultBase
    {
        var connection = new Connection(_options.Host, _options.Port);
        if (_options.SessionTimeout.HasValue) connection.Timeout = _options.SessionTimeout.Value;
        var messagePack = command.ToString(_options.ProtocolVersion, _options.User);
        var result = await connection.SendAsync(messagePack);
        return (T)Activator.CreateInstance(typeof(T), result);
    }
}