using Kooboo.Data.Models;
using Kooboo.Dom.CSS;
using System.Collections.Generic;
using System.Linq; 

namespace Kooboo.Web.Lighthouse.CssMinimizer
{
    public class CssParser
    { 
        public static CssParser instance { get; set; }= new CssParser();

        public List<CssPart> ParseMediaParts(string cssBody)
        {
            List<CssPart> parts = new List<CssPart>();

            var style = Kooboo.Dom.CSSParser.ParseCSSStyleSheet(cssBody);
  
            foreach (var item in style.cssRules.item)
            {
                if (item is CSSMediaRule)
                {
                    var media = item as CSSMediaRule;
                    if (media != null)
                    {
                        CssPart part = new CssPart();
                        part.StartIndex = media.StartIndex;
                        part.EndIndex = media.EndIndex;
                        part.ConditionText = media.conditionText;

                        if (media.media != null)
                        {
                            foreach (var m in media.media.item)
                            {
                                part.Media.Add(m);
                            }
                        }

                        parts.Add(part);
                    }
                }
            }

            return parts;
        }

        private List<CSSMediaRule> GetMediaRules(CSSStyleSheet style)
        {
            List<CSSMediaRule> rules = new List<CSSMediaRule>();
             
            foreach (var item in style.cssRules.item)
            {
                if (item is CSSMediaRule)
                {
                    var media = item as CSSMediaRule;
                    rules.Add(media); 
                }
            } 
            return rules; 
        }

        //only matters are the min-width, and maxwidth. 
        public WidthSetting ParseWidthSetting(string conditionText)
        {
            if (conditionText == null)
            {
                return null;
            }

            conditionText = conditionText.ToLower().Trim();

            WidthSetting setting = new WidthSetting();

            var minIndex = conditionText.IndexOf("min-width");

            if (minIndex >= 0)
            {
                setting.MinWidth = consumeValue(conditionText, minIndex + "min-width".Length);
            }
            else
            {
                minIndex = conditionText.IndexOf("min-device-width");
                if (minIndex >= 0)
                {
                    setting.MinWidth = consumeValue(conditionText, minIndex + "min-device-width".Length);
                }

            }

            var maxIndex = conditionText.IndexOf("max-width");

            if (maxIndex >= 0)
            {
                setting.MaxWidth = consumeValue(conditionText, maxIndex + "max-width".Length);
            }
            else
            {
                maxIndex = conditionText.IndexOf("max-device-width");
                if (maxIndex >= 0)
                {
                    setting.MaxWidth = consumeValue(conditionText, maxIndex + "max-device-width".Length);
                }
            }
            return setting;

        }

        private string consumeValue(string originalText, int startIndex)
        {
            string value = null;
            int len = originalText.Length;

            int afterComma = 0;
            // consume : 
            for (int i = startIndex; i < len; i++)
            {
                if (originalText[i] == ':')
                {
                    afterComma = i + 1;
                    break;
                }
            }

            bool beforeValueState = true;
            bool InValueState = false;


            if (afterComma > 0)
            {
                for (int i = afterComma; i < len; i++)
                {
                    var current = originalText[i];
                    if (beforeValueState)
                    {
                        if (char.IsLetterOrDigit(current))
                        {
                            beforeValueState = false;
                            InValueState = true;
                            i--;
                        }
                    }
                    else if (InValueState)
                    {
                        //it may write as 120 px; 
                        if (char.IsLetterOrDigit(current))
                        {
                            value += current;
                        }
                        else
                        {
                            InValueState = false;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }

            return value?.Trim();
        }

    }
}
