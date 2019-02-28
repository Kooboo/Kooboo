//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom.CSS;
using System.Text.RegularExpressions;

namespace Kooboo.Dom.CSSParser
{
    /// <summary>
    /// Convert antlr models to W3C style
    /// </summary>
    public static class ModelConverter
    {
        public static CSSDeclaration ConvertToCSSDeclaration(AntlrModel.Declaration declaration)
        {
            return new CSSDeclaration()
            {
                propertyname = declaration.Property,
                value = declaration.Expr.Serialize(),
                important = declaration.Prio
            };
        }

        public static CSSPageRule ConvertToCSSPageRule(AntlrModel.PageRule page)
        {
            var pageRule = new CSSPageRule();
            pageRule.selectorText = page.SerializeSelectorList();
            pageRule.cssText = page.Serialize();
            var style = new CSSStyleDeclaration();
            style.cssText = page.PageBody.SerializeDeclaration();
            foreach (var decl in page.PageBody.AllDeclarations())
            {
                style.item.Add(ConvertToCSSDeclaration(decl));
            }
            pageRule.style = style;
            return pageRule;
        }

        public static CSSMediaRule ConvertToCSSMediaRule(AntlrModel.Media media)
        {
            var mediaRule = new CSSMediaRule();
            mediaRule.media = ConvertToMediaList(media);
            mediaRule.cssText = media.Serialize();
            mediaRule.selectorText = media.SerializeMedium();
            mediaRule.conditionText = "@media";
            mediaRule.cssRules.appendRule(ConvertToCSSStyleRule(media.RuleSet));
            return mediaRule;
        }

        public static CSSStyleRule ConvertToCSSStyleRule(AntlrModel.RuleSet ruleset)
        {
            var rule = new CSSStyleRule();
            rule.cssText = ruleset.Serialize();
            foreach (var selector in ruleset.Selectors)
            {
                if (selector.SimpleSelector != null)
                {
                    rule.selectors.Add(new simpleSelector()
                    {
                        Type = (enumSimpleSelectorType)selector.SimpleSelector.GetSelectorType(),
                        wholeText = selector.SimpleSelector.Serialize()
                    });
                }
                if (selector.CombinationSelectors != null && selector.CombinationSelectors.Count != 0)
                {
                    foreach (var cs in selector.CombinationSelectors)
                    {
                        rule.selectors.Add(new simpleSelector()
                        {
                            Type = enumSimpleSelectorType.combinator,
                            wholeText = cs.Serialize()
                        });
                    }
                }

            }
            return rule;
        }

        public static CSSImportRule ConvertToCSSImportRule(AntlrModel.Import import, string basePath = "")
        {
            var importRule = new CSSImportRule();
            importRule.cssText = import.Serialize();
            var _href = PathHelper.ParseHref(import.Value);
            importRule.href = PathHelper.combine(basePath,_href);
            foreach (var m in import.Medium)
            {
                importRule.media.appendMedium(m);
            }
            //提供stylesheet.href属性，在访问cssText时就会去下载远程css文件。
            //一解析就下载也浪费性能，如果需要完整的stylesheet对象,可以：
            //importRule.stylesheet=ConvertToCSSStyleSheet(importRule.stylesheet.cssText)
            importRule.stylesheet = new CSSStyleSheet()
            {
                href = importRule.href
            };
            return importRule;
        }

        private static string ParseHref(string importValue, string basePath = "")
        {
            //todo: re 
            basePath = basePath.Trim();
            try
            {
                var s = importValue.Trim();
                s = s.Replace("url(", "").Replace(")","").Replace(";","");
                s = s.Substring(1, s.Length - 2);
                return PathHelper.combine(basePath, s);
            }
            catch
            {
                return importValue;
            }
        }

        public static MediaList ConvertToMediaList(AntlrModel.Media media)
        {
            var list = new MediaList();
            foreach (var m in media.Medium)
            {
                list.appendMedium(m);
            }
            return list;
        }

        public static CSSStyleSheet ConvertToCSSStyleSheet(AntlrModel.Stylesheet stylesheet, string basePath = "")
        {
            var cssStyleSheet = new CSSStyleSheet();
            cssStyleSheet.cssText = stylesheet.Serialize();
            cssStyleSheet.charsetRule = ConvertToCSSCharsetRule(stylesheet.CharSet);
            foreach (var import in stylesheet.Imports)
            {
                var importRule = ConvertToCSSImportRule(import, basePath);
                cssStyleSheet.cssRules.appendRule(importRule);
            }
            foreach (var bodyset in stylesheet.BodyList)
            {
                var rule = new CSSRule();
                if (bodyset is AntlrModel.RuleSet)
                {
                    rule = ConvertToCSSStyleRule(bodyset as AntlrModel.RuleSet);
                }
                else if (bodyset is AntlrModel.PageRule)
                {
                    rule = ConvertToCSSPageRule(bodyset as AntlrModel.PageRule);
                }
                else if (bodyset is AntlrModel.Media)
                {
                    rule = ConvertToCSSMediaRule(bodyset as AntlrModel.Media);
                }
                cssStyleSheet.cssRules.appendRule(rule);
            }
            return cssStyleSheet;
        }

        public static CSSCharsetRule ConvertToCSSCharsetRule(AntlrModel.CharSet charset)
        {
            var charsetRule = new CSSCharsetRule();
            charsetRule.encoding = new Regex(@"['|""]").Replace(charset.Name, "");
            charsetRule.cssText = charset.Serialize();
            return charsetRule;
        }

        public static CSSMarginRule ConvertToCSSMarginRule(AntlrModel.PageMarginBox marginBox)
        {
            var marginRule = new CSSMarginRule();
            marginRule.cssText = marginBox.Serialize();
            marginRule.name = marginBox.Margin_sym;
            foreach (var item in marginBox.Declaration)
            {
                marginRule.style.item.Add(ConvertToCSSDeclaration(item));
            }
            return marginRule;
        }
    }
}
