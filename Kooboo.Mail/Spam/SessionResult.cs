using System;

namespace Kooboo.Mail.Spam
{
    public record SessionResult
    {
        public TestResult RDNS { get; set; }

        public TestResult SPF { get; set; }

        public TestResult DKIM { get; set; }

        public bool IsContact { get; set; }

        public bool AllFailed
        {
            get
            {
                if (RDNS == TestResult.Failed && SPF == TestResult.Failed && DKIM == TestResult.Failed)
                {
                    return true;
                }
                return false;
            }
        }

        public bool AllSuccess
        {
            get
            {
                if (RDNS == TestResult.Pass && SPF == TestResult.Pass && DKIM == TestResult.Pass)
                {
                    return true;
                }
                return false;
            }
        }

        public string ToHeaderLine()
        {
            string xheader = "k-auth:";

            xheader += "SPF=" + this.SPF.ToString();

            xheader += ",DKIM=" + this.DKIM.ToString();

            xheader += ",RDNS=" + this.RDNS.ToString();

            return xheader;
        }

        public static SessionResult FromMsgBody(string messagebody)
        {
            var index = messagebody.IndexOf("\r\nk-auth:");

            if (index > -1)
            {
                var end = messagebody.IndexOf("\r\n", index + 1);

                if (end > index)
                {
                    SessionResult result = new SessionResult();

                    int leng = "\r\nk-auth:".Length;

                    var line = messagebody.Substring(index + leng, end - index - leng);

                    var parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in parts)
                    {
                        var sep = item.IndexOf("=");

                        if (sep > -1)
                        {
                            var testResult = item.Substring(sep + 1).Trim();

                            var enumResult = Kooboo.Lib.Helper.EnumHelper.GetEnum<TestResult>(testResult);

                            var itemKey = item.Substring(0, sep).Trim();

                            if (itemKey == "SPF")
                            {
                                result.SPF = enumResult;
                            }
                            else if (itemKey == "DKIM")
                            {
                                result.DKIM = enumResult;
                            }
                            else if (itemKey == "RDNS")
                            {
                                result.RDNS = enumResult;
                            }

                        }


                    }

                    return result;

                }

            }


            return null;
        }
    }
}
