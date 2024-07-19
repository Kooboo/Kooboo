using System;

namespace SpamassassinNet;

public class ClientOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 783;
    public string ProtocolVersion { get; set; } = "1.5";
    public string? User { get; set; }

    public TimeSpan? SessionTimeout { get; set; }
}