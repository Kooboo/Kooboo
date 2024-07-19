namespace Kooboo.Lib.Utilities.UAParser.Platform
{
    public interface IPlatformMatcher
    {
        string PlatformName { get; }

        int Preference { get; }

        bool Match(string userAgentString);
    }
}

