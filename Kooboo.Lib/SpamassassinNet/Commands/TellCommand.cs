using System.Collections.Generic;
using Kooboo.ApiMarket;

namespace SpamassassinNet.Commands;

public class TellCommand : CommandBase<BasicResult>
{
    protected override string Name => "TELL";
    protected override string Body { get; }
    public MessageClass? MessageClass { get; set; }
    public DatabaseKind? Set { get; set; }
    public DatabaseKind? Remove { get; set; }

    public TellCommand(string body)
    {
        Body = body;
    }

    protected override IEnumerable<string> AppendHeaders()
    {
        var result = new List<string>();

        if (MessageClass.HasValue)
        {
            result.Add($"Message-class: {MessageClass.Value.ToString().ToLower()}");
        }

        if (Set.HasValue)
        {
            var set = GetDatabaseKindString(Set.Value);
            result.Add($"Set: {set.ToLower()}");
        }

        if (Remove.HasValue)
        {
            var remove = GetDatabaseKindString(Remove.Value);
            result.Add($"Set: {remove.ToLower()}");
        }

        return result;
    }

    private static string GetDatabaseKindString(DatabaseKind databaseKind)
    {
        var list = new List<string>();

        if (databaseKind.HasFlag(DatabaseKind.Local))
        {
            list.Add(DatabaseKind.Local.ToString());
        }

        if (databaseKind.HasFlag(DatabaseKind.Remote))
        {
            list.Add(DatabaseKind.Remote.ToString());
        }

        return string.Join(", ", list);
    }
}