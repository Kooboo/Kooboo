using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using System.Linq;

namespace Kooboo.Web.Lighthouse.CssMinimizer
{
    public class CssSpliter
    {

        public static CssSpliter instance { get; set; } = new CssSpliter();

        public string Render(RenderContext context, System.Guid Id, string body)
        {
            if (context.WebSite.EnableCssSplitByMedia)
            {

                body = CssReducer.instance.Render(context, Id, body); 

                WidthSetting setting = new();
                if (context.IsMobile)
                {
                    setting.MaxWidth = context.WebSite.MobileMaxWidth;  
                }
                else
                {
                    setting.MinWidth = context.WebSite.DesktopMinWidth;
                }

                return GenerateCssText(body, setting);
            }

            return null;
        }

        public string GenerateCssText(string CssBody, WidthSetting setting)
        {
            var parser = new CssParser();

            var parts = parser.ParseMediaParts(CssBody);

            if (parts == null || parts.Count == 0)
            {
                return CssBody;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
             
            int totalLen = CssBody.Length;
            int currentIndex = 0;

            foreach (var item in parts.OrderBy(o => o.StartIndex))
            {
                if (!string.IsNullOrEmpty(item.ConditionText))
                {
                    var widthsetting = CssParser.instance.ParseWidthSetting(item.ConditionText);

                    if (!IsMatch(setting, widthsetting))
                    {
                        var len = item.StartIndex - currentIndex;

                        if (len > 0)
                        {
                            var current = CssBody.Substring(currentIndex, item.StartIndex - currentIndex);

                            sb.Append(current); 
                        }
                        currentIndex = item.EndIndex + 1;
                    }
                }
            }

            if (currentIndex == 0)
            {
                return CssBody;
            }
            else if (currentIndex < totalLen - 1)
            {
                sb.Append(CssBody.Substring(currentIndex, totalLen - currentIndex));
            }

            return sb.ToString();
        }

        public bool IsMatch(WidthSetting setting, WidthSetting mediaValue)
        {
            if (mediaValue == null || setting == null)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(mediaValue.MinWidth))
            {
                if (!string.IsNullOrWhiteSpace(setting.MaxWidth))
                {
                    if (BiggerThan(mediaValue.MinWidth, setting.MaxWidth))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(mediaValue.MaxWidth))
            {
                if (!string.IsNullOrWhiteSpace(setting.MinWidth))
                {

                    if (BiggerThan(setting.MinWidth, mediaValue.MaxWidth))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return true;  // default must all return.
        }

        public bool BiggerThan(string source, string target)
        {
            var numberA = GetDigit(source);
            var numberB = GetDigit(target);

            var dimiA = GetAZ(source);
            var dimiB = GetAZ(target);

            if (dimiA == null || dimiB == null)
            {
                return false;
            }
            if (dimiA.ToLower() != dimiB.ToLower())
            {
                return false;
            }

            return numberA > numberB;
        }

        private int GetDigit(string input)
        {
            if (input == null)
            {
                return 0;
            }
            string digitstring = null;
            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];
                if (char.IsDigit(current))
                {
                    digitstring += current;
                }
            }

            if (digitstring == null)
            {
                return 0;
            }
            else
            {
                return System.Convert.ToInt32(digitstring);
            }
        }

        private string GetAZ(string input)
        {
            if (input == null)
            {
                return null;
            }
            string dimision = null;
            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];
                if (!char.IsDigit(current))
                {
                    dimision += current;
                }
            }

            return dimision;

        }


    }





}
