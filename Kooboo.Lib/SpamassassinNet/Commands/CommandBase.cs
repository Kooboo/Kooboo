using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public abstract class CommandBase<T> where T : ResultBase
{
    protected abstract string Name { get; }
    protected abstract string Body { get; }

    protected virtual IEnumerable<string> AppendHeaders()
    {
        return Array.Empty<string>();
    }

    public string ToString(string version, string? user = null)
    {
        var commandBuilder = new StringBuilder();
        commandBuilder.Append($"{Name} SPAMC/{version}\r\n");
        commandBuilder.Append($"Content-length: {Body.Length}\r\n");

        if (user != null)
        {
            commandBuilder.Append($"User: {user}\r\n");
        }

        var appendedHeaders = AppendHeaders();

        foreach (var header in appendedHeaders)
        {
            commandBuilder.Append($"{header}\r\n");
        }

        commandBuilder.Append($"\r\n");
        commandBuilder.Append(Body);
        return commandBuilder.ToString();
    }
}