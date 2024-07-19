using System;

namespace Kooboo.Api
{
    public static class ApiVersion
    {
        public const string V1 = nameof(V1);
        public const string V2 = nameof(V2);

        public static string GetVersion(string version)
        {
            if (V2.Equals(version, StringComparison.OrdinalIgnoreCase)) return V2;
            return V1;
        }

        public static bool IsVersion(string version)
        {
            return V1.Equals(version, StringComparison.OrdinalIgnoreCase) ||
                   V2.Equals(version, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class ApiVersionAttribute : Attribute
    {
        public string Version { get; }
        public ApiVersionAttribute(string version)
        {
            Version = version;
        }
    }
}