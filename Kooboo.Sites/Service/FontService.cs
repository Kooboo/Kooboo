//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Service.Font;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Service
{
    public class FontService
    {
        private static int FontHash = Lib.Security.Hash.ComputeIntCaseSensitive("font");
        private static int FontFamilyHash = Lib.Security.Hash.ComputeIntCaseSensitive("font-family");

        //public static List<FontFamilyDeclaration> GetFontDeclarations(SiteDb SiteDb, Guid PageId = default(Guid))
        //{
        //    List<Guid> AllIds = null;
        //    List<CmsCssDeclaration> AllDeclarations = null;
        //    List<CmsCssRule> AllRules = null;

        //    if (PageId == default(Guid))
        //    {
        //        AllRules = SiteDb.CssRules.All();
        //        AllDeclarations = SiteDb.CssDeclarations.Query.Where(o => SearchFontDeclaration(o.PropertyNameHash)).SelectAll();
        //    }
        //    else
        //    {
        //        AllIds = SiteDb.Pages.GetRelatedOwnerObjectIds(PageId);
        //        var all = SiteDb.CssDeclarations.Query.Where(o => SearchFontDeclaration(o.PropertyNameHash)).SelectAll();
        //        AllDeclarations = all.Where(o => AllIds.Contains(o.OwnerObjectId)).ToList();
        //        AllRules = SiteDb.CssRules.Query.WhereIn<Guid>(o => o.OwnerObjectId, AllIds).SelectAll();
        //    }

        //    Func<Guid, CmsCssRule> GetRule = (ruleid) =>
        //    {
        //        var find = AllRules.Find(o => o.Id == ruleid);
        //        if (find != null)
        //        {
        //            return find;
        //        }
        //        return SiteDb.CssRules.Get(ruleid);
        //    };

        //    List<FontFamilyDeclaration> result = new List<FontFamilyDeclaration>();

        //    foreach (var item in AllDeclarations)
        //    {
        //        string fontfamily = string.Empty;

        //        if (item.PropertyNameHash == FontHash)
        //        {
        //            var longfont = Sites.Service.FontService.ParseFont(item.Value);
        //            fontfamily = longfont.FontFamily;
        //        }
        //        else if (!string.IsNullOrEmpty(item.Value))
        //        {
        //            fontfamily = item.Value.Trim();
        //        }

        //        if (!string.IsNullOrEmpty(fontfamily))
        //        {
        //            var rule = GetRule(item.CmsCssRuleId);
        //            if (rule != null)
        //            {
        //                FontFamilyDeclaration decl = new FontFamilyDeclaration();
        //                decl.FontFamily = fontfamily;
        //                decl.CmsDeclarationId = item.Id;
        //                decl.CmsCssRuleId = item.CmsCssRuleId;
        //                decl.Selector = rule.SelectorText;
        //                decl.PropertyName = item.PropertyName;
        //                decl.Value = item.Value;
        //                decl.ParentStyleId = rule.ParentStyleId;
        //                decl.IsInline = rule.IsInline;
        //                decl.KoobooId = rule.KoobooId;
        //                decl.OwnerConstType = rule.OwnerObjectConstType;
        //                decl.OwnerObjectId = rule.OwnerObjectId;
        //                result.Add(decl);
        //            }
        //        }
        //    }

        //    return result;
        //}

        private static bool SearchFontDeclaration(int propertyhash)
        {
            return (propertyhash == FontFamilyHash || propertyhash == FontHash);
        }

        public static LongFont ParseFont(string shortHandFont)
        {
            LongFont result = new LongFont();
            var tokenizer = new FontTokenizer(shortHandFont);
            string token = tokenizer.ConsumeNextTrim();

            var trylist = TryList;
            int count = trylist.Count();

            for (int i = 0; i < count; i++)
            {
                var item = trylist[i];
                string value = item.Method(token);
                if (!string.IsNullOrEmpty(value))
                {
                    AssignValue(result, item.State, value);
                    token = tokenizer.ConsumeNextTrim();
                    continue;
                }
                else
                {
                    for (int j = i + 1; j < count; j++)
                    {
                        var nextitem = trylist[j];
                        string nextvalue = nextitem.Method(token);
                        if (!string.IsNullOrEmpty(nextvalue))
                        {
                            AssignValue(result, nextitem.State, nextvalue);
                            token = tokenizer.ConsumeNextTrim();
                            i = i - 1;
                            goto nextloop;
                        }
                    }
                    break;
                }

            nextloop:
                {
                    continue;
                }
            }

            while (!string.IsNullOrEmpty(token))
            {
                result.FontFamilyList.Add(token.Trim());
                token = tokenizer.ConsumeNextTrim();
            }

            return result;
        }

        private static void AssignValue(LongFont font, TryState state, string value)
        {
            switch (state)
            {
                case TryState.Stretch:
                    if (font.FontStretch == "normal")
                    {
                        font.FontStretch = value;
                    }
                    break;

                case TryState.Style:
                    if (font.FontStyle == "normal")
                    {
                        font.FontStyle = value;
                    }
                    break;

                case TryState.Variant:
                    if (font.FontVariant == "normal")
                    {
                        font.FontVariant = value;
                    }
                    break;

                case TryState.Weight:
                    {
                        if (font.FontWeight == "normal")
                        {
                            font.FontWeight = value;
                        }
                    }
                    break;

                case TryState.FontSize:
                    {
                        if (font.FontSize == "medium")
                        {
                            font.FontSize = value;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private static string TryStyle(string input)
        {
            //The ‘font-style’ property allows italic or oblique faces to be selected.
            //Name: font - style //Value: normal | italic | oblique
            var lower = input.ToLower();
            if (lower == "normal" || lower == "italic" || lower == "oblique")
            {
                return input;
            }
            return null;
        }

        private static string TryVariant(string input)
        {
            // 15.5 Small - caps: the 'font-variant' property
            //'font-variant'
            //Value: normal | small-caps | inherit
            //Initial: normal
            //Applies to: all elements
            //Inherited: yes
            //Percentages:N/A
            string lower = input.ToLower();
            if (lower == "normal" || lower == "small-caps")
            {
                return input;
            }
            return null;
        }

        private static string TryWeight(string input)
        {
            //'font-weight' is matched next, it will never fail. (See 'font-weight' below.)
            // TODO: check what is the meanning of "it will never fail".
            //	normal | bold | bolder | lighter | 100 | 200 | 300 | 400 | 500 | 600 | 700 | 800 | 900 |
            if (input == "100" || input == "200" || input == "300" || input == "400" || input == "500" || input == "600" || input == "700" || input == "800" || input == "900")
            {
                return input;
            }

            string lower = input.ToLower();
            if (lower == "normal" || lower == "bold" || lower == "bolder" || lower == "lighter")
            {
                return input;
            }

            return null;
        }

        private static string TryFontSize(string input)
        {
            // Name: font - size
            //Value:  < absolute - size > | < relative - size > | < length > | < percentage >

            char first = input[0];
            // length or percentage...
            if (CharHelper.isAsciiDigit(first))
            {
                return input;
            }
            string lower = input.ToLower();
            // < absolute - size >
            //An < absolute - size > keyword refers to an entry in a table of font sizes computed and kept by the user agent. Possible values are:
            //[xx-small | x-small | small | medium | large | x-large | xx-large]

            // A < relative - size > keyword is interpreted relative to the table of font sizes //and the computed ‘font - size’ of the parent element. Possible values are:[larger | smaller]
            if (input == "xx-small" || input == "x-small" || input == "small" || input == "medium" || input == "large" || input == "x-large" || input == "xx-large" || input == "larger" || input == "smaller")
            {
                return input;
            }

            int slashIndex = -1;
            slashIndex = input.IndexOf("/");
            if (slashIndex == -1)
            {
                slashIndex = input.IndexOf("/");
            }

            if (slashIndex > -1)
            {
                string fontsizevalue = input.Substring(0, slashIndex);
                string back = TryFontSize(fontsizevalue);
                if (!string.IsNullOrEmpty(back))
                {
                    return input;
                }
            }

            return null;
        }

        private static string TryStretch(string input)
        {
            //Name: font - stretch Value: normal | ultra-condensed | extra-condensed | condensed | semi-condensed | semi-expanded | expanded | extra-expanded | ultra-expanded
            var lower = input.ToLower();
            // if (lower == "normal" || lower == "ultra-condensed" || lower == "extra-condensed" || lower == "condensed")
            if (lower == "ultra-condensed" || lower == "extra-condensed" || lower == "condensed")
            {
                return input;
            }
            return null;
        }

        private static List<StateTry> _trylist;

        private static List<StateTry> TryList
        {
            get
            {
                if (_trylist == null)
                {
                    _trylist = new List<StateTry>
                    {
                        new StateTry {State = TryState.Stretch, Method = FontService.TryStretch},
                        new StateTry {State = TryState.Style, Method = FontService.TryStyle},
                        new StateTry {State = TryState.Variant, Method = FontService.TryVariant},
                        new StateTry {State = TryState.Weight, Method = FontService.TryWeight},
                        new StateTry {State = TryState.FontSize, Method = FontService.TryFontSize}
                    };
                }
                return _trylist;
            }
        }

        private class StateTry
        {
            public TryState State { get; set; }

            public Func<string, string> Method { get; set; }
        }

        public enum TryState
        {
            Stretch = 1,
            Style = 2,
            Variant = 3,
            Weight = 4,
            FontSize = 5
        }
    }
}

namespace Kooboo.Sites.Service.Font
{
    public class LongFont
    {
        public string FontStyle { get; set; } = "normal";
        public string FontVariant { get; set; } = "normal";
        public string FontWeight { get; set; } = "normal";
        public string FontStretch { get; set; } = "normal";
        public string FontSize { get; set; } = "medium";
        public string LineHeight { get; set; } = "normal";

        private string _fontfamily;

        public string FontFamily
        {
            get
            {
                if (string.IsNullOrEmpty(_fontfamily) && this.FontFamilyList != null && this.FontFamilyList.Count() > 0)
                {
                    _fontfamily = string.Join(",", this.FontFamilyList.ToArray());
                }
                return _fontfamily;
            }
            set { _fontfamily = value; }
        }

        private List<string> _fontfamilylist;

        public List<string> FontFamilyList
        {
            get
            {
                if (_fontfamilylist == null)
                {
                    if (!string.IsNullOrEmpty(this._fontfamily))
                    {
                        _fontfamilylist = new List<string>(this._fontfamily.Split(','));
                    }
                    else
                    {
                        _fontfamilylist = new List<string>();
                    }
                }
                return _fontfamilylist;
            }
            set
            {
                _fontfamilylist = value;
            }
        }

        //font-style: normal
        //font-variant: normal
        //font-weight: normal
        //font-stretch: normal
        //font-size: medium
        //line-height: normal
    }

    public class FontTokenizer
    {
        private string Value { get; set; }
        private int Index { get; set; }
        private int Len { get; set; }
        private List<char> _buffer = new List<char>();

        private bool IsEof { get; set; }

        public FontTokenizer(string propertyValue)
        {
            this.Value = propertyValue;
            this.Index = 0;
            this.Len = propertyValue.Length;
            if (string.IsNullOrEmpty(propertyValue))
            {
                this.Len = -1;
            }
        }

        public string ConsumeNext()
        {
            if (this.IsEof)
            {
                return null;
            }

            for (int i = Index; i < Len; i++)
            {
                var currentchar = Value[i];

                if (currentchar == ';')
                {
                    //end...
                    string value = null;
                    if (_buffer.Count > 0)
                    {
                        value = new string(_buffer.ToArray());
                        _buffer.Clear();
                    }
                    this.IsEof = true;
                    return value;
                }

                if (currentchar == '\'' || currentchar == '"')
                {
                    if (_buffer.Any())
                    {
                        string value = new string(_buffer.ToArray());
                        _buffer.Clear();
                        this.Index = i;
                        return value;
                    }
                    int nextend = -1;

                    for (int j = i + 1; j < Len; j++)
                    {
                        if (Value[j] == currentchar)
                        {
                            nextend = j;
                            break;
                        }
                    }
                    if (nextend > -1)
                    {
                        string nextpart = Value.Substring(i, nextend - i + 1);
                        this.Index = nextend + 1;
                        return nextpart;
                    }
                    else
                    {
                        string allToEnd = Value.Substring(i);
                        this.IsEof = true;
                        return allToEnd;
                    }
                }
                else if (CharHelper.isSpaceCharacters(currentchar) || currentchar == ',')
                {
                    this.Index = i + 1;
                    if (_buffer.Any())
                    {
                        string output = new string(_buffer.ToArray());
                        _buffer.Clear();
                        return output;
                    }
                    else
                    {
                        return ConsumeNext();
                    }
                }
                else
                {
                    _buffer.Add(currentchar);
                }
            }
            this.IsEof = true;
            if (_buffer.Any())
            {
                return new string(_buffer.ToArray());
            }
            return null;
        }

        public string ConsumeNextTrim()
        {
            var next = this.ConsumeNext();
            return string.IsNullOrEmpty(next) ? null : next.Trim();
        }
    }
}