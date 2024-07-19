using System;

namespace SpamassassinNet;

[Flags]
public enum DatabaseKind
{
    Local = 0b1,
    Remote = 0b10
}