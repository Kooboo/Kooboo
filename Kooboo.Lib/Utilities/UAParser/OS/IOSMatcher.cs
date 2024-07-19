namespace Kooboo.Lib.Utilities.UAParser.OS
{
    public interface IOSMatcher
    {
        string OSName { get; }

        bool Match(string userAgentString);
    }
}

