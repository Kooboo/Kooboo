using System.Linq;
using Kooboo.Lib.Utilities.UAParser.Device;
using Kooboo.Lib.Utilities.UAParser.MailApp;
using Kooboo.Lib.Utilities.UAParser.OS;
using Kooboo.Lib.Utilities.UAParser.Platform;
using Kooboo.Lib.Utilities.UAParser.WebBrowser;

namespace Kooboo.Lib.Utilities.UAParser
{
    public static class UAParser
    {
        private static PlatformMatcherExecutor _platformMatcher = new PlatformMatcherExecutor();
        private static OSMatcherExecutor _osMatcherExecutor = new OSMatcherExecutor();
        private static DeviceExecutor _deviceExecutor = new DeviceExecutor();
        private static MailAppExecutor _mailAppExecutor = new MailAppExecutor();
        private static WebBrowserExecutor _webBrowserExecutor = new WebBrowserExecutor();

        public static ClientInfo Match(string userAgentString, bool CheckBrowserFirst = false)
        {
            if (userAgentString == null)
            {
                return null;
            }

            var userAgentCoreInfo = GetCoreString(userAgentString);

            if (string.IsNullOrWhiteSpace(userAgentCoreInfo))
            {
                return null;
            }

            var platform = _platformMatcher.Match(userAgentCoreInfo);

            var os = _osMatcherExecutor.Match(userAgentCoreInfo);

            var device = _deviceExecutor.Match(userAgentCoreInfo);

            ApplicationInfo application = null;

            if (CheckBrowserFirst)
            {
                application = _webBrowserExecutor.Match(userAgentString);
                if (application != null)
                {
                    application.IsWebBrowser = true;
                }
                else
                {
                    application = _mailAppExecutor.Match(userAgentString);
                }
            }
            else
            {
                application = _mailAppExecutor.Match(userAgentString);
                if (application == null)
                {
                    application = _webBrowserExecutor.Match(userAgentString);
                    if (application != null)
                    {
                        application.IsWebBrowser = true;
                    }
                }
            }

            if (application == null)
            {
                if (userAgentString != null && userAgentString.Length < 30)  // like  Mozilla/5.0;
                {
                    application = new ApplicationInfo() { Name = userAgentString };
                }
                else
                {
                    var applicationResult = string.Empty;
                    if (userAgentString.Contains(' '))
                    {
                        var uaFirstString = userAgentString.Split(' ')?.FirstOrDefault();
                        if (string.IsNullOrEmpty(uaFirstString))
                            applicationResult = "Unknown";

                        applicationResult = uaFirstString;
                    }
                    else
                    {
                        applicationResult = "Unknown";
                    }

                    application = new ApplicationInfo() { Name = applicationResult };
                }
            }

            return platform != null && os != null ? new ClientInfo(platform, os, application, device) : null;

        }



        private static string GetCoreString(string userAgentString)
        {
            var indexLeft = userAgentString.IndexOf('(');
            var indexRight = userAgentString.IndexOf(')');
            if (indexLeft < 0 || indexRight < 0)
                return userAgentString;

            if (indexLeft > indexRight)
            {
                return userAgentString;
            }

            return userAgentString.Substring(indexLeft, indexRight - indexLeft);
        }

        public static bool IsMobile(string UserAgent)
        {
            var userAgentCoreInfo = GetCoreString(UserAgent);

            var platform = _platformMatcher.Match(userAgentCoreInfo);

            var OS = _osMatcherExecutor.Match(userAgentCoreInfo);

            if (platform == null)
            {
                return false;
            }

            if (platform == "Mobile")
            {
                return true;
            }
            else if (platform == "Desktop")
            {
                return false;
            }

            if (OS == null)
            {
                return false;
            }

            if (OS == "IOS")
            {
                return true;
            }

            if (OS == "Mac" || OS == "Windows" || OS == "Linux")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

    public class ApplicationInfo
    {
        public ApplicationInfo() { }

        public ApplicationInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public bool IsWebBrowser { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            if (Name != null)
            {
                return Version == null ? Name : Name + "-" + Version;
            }
            return null;
        }
    }


}

