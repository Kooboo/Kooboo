//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Service
{
    public class ColorService
    {
        // search and return the color code...
        // create own tokenizer for fastest performance...
        // This was written to search for color for the highest performance...
        public static string SearchColor(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            List<char> buffer = new List<char>();

            int len = input.Length;
            for (int i = 0; i < len; i++)
            {
                var currentchar = input[i];
                if (currentchar == '#')
                {
                    if ((i + 6) < len)
                    {
                        if (CharHelper.isAsciiHexDigit(input[i + 6]) && CharHelper.isAsciiHexDigit(input[i + 5]) && CharHelper.isAsciiHexDigit(input[i + 4]) && CharHelper.isAsciiHexDigit(input[i + 3]) && CharHelper.isAsciiHexDigit(input[i + 2]) && CharHelper.isAsciiHexDigit(input[i + 1]))
                        {
                            return input.Substring(i, 7);
                        }
                    }
                    else if ((i + 3) < len)
                    {
                        if (CharHelper.isAsciiHexDigit(input[i + 3]) && CharHelper.isAsciiHexDigit(input[i + 2]) && CharHelper.isAsciiHexDigit(input[i + 1]))
                        {
                            return input.Substring(i, 4);
                        }
                    }
                }
                else if (CharHelper.IsAscii(currentchar))
                {
                    buffer.Add(currentchar);
                }
                else
                {
                    string output = new string(buffer.ToArray());
                    buffer.Clear();

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        string lower = output.ToLower().Trim();
                        if (lower == "rgb" || lower == "rgba" || lower == "hsl" || lower == "hsla")
                        {
                            // find the next )
                            int nextbracket = -1;
                            for (int j = i; j < len; j++)
                            {
                                if (input[j] == ')')
                                {
                                    nextbracket = j;
                                    break;
                                }
                            }
                            if (nextbracket > -1)
                            {
                                string nextpart = input.Substring(i, nextbracket - i + 1);
                                return output + nextpart;
                            }
                        }
                        else if (ColorNameSet.Contains(output))
                        {
                            return output;
                        }
                    }
                }
            }

            return null;
        }

        private static HashSet<string> _nameset;

        public static HashSet<string> ColorNameSet
        {
            get
            {
                if (_nameset == null)
                {
                    _nameset = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "Black",
                        "Navy",
                        "DarkBlue",
                        "MediumBlue",
                        "Blue",
                        "DarkGreen",
                        "Green",
                        "Teal",
                        "DarkCyan",
                        "DeepSkyBlue",
                        "DarkTurquoise",
                        "MediumSpringGreen",
                        "Lime",
                        "SpringGreen",
                        "Aqua",
                        "Cyan",
                        "MidnightBlue",
                        "DodgerBlue",
                        "LightSeaGreen",
                        "ForestGreen",
                        "SeaGreen",
                        "DarkSlateGray",
                        "LimeGreen",
                        "MediumSeaGreen",
                        "Turquoise",
                        "RoyalBlue",
                        "SteelBlue",
                        "DarkSlateBlue",
                        "MediumTurquoise",
                        "Indigo",
                        "DarkOliveGreen",
                        "CadetBlue",
                        "CornflowerBlue",
                        "MediumAquaMarine",
                        "DimGray",
                        "DimGrey",
                        "SlateBlue",
                        "OliveDrab",
                        "SlateGray",
                        "LightSlateGray",
                        "MediumSlateBlue",
                        "LawnGreen",
                        "Chartreuse",
                        "Aquamarine",
                        "Maroon",
                        "Purple",
                        "Olive",
                        "Gray",
                        "SkyBlue",
                        "LightSkyBlue",
                        "BlueViolet",
                        "DarkRed",
                        "DarkMagenta",
                        "SaddleBrown",
                        "DarkSeaGreen",
                        "LightGreen",
                        "MediumPurple",
                        "DarkViolet",
                        "PaleGreen",
                        "DarkOrchid",
                        "YellowGreen",
                        "Sienna",
                        "Brown",
                        "DarkGray",
                        "LightBlue",
                        "GreenYellow",
                        "PaleTurquoise",
                        "LightSteelBlue",
                        "PowderBlue",
                        "FireBrick",
                        "DarkGoldenRod",
                        "MediumOrchid",
                        "RosyBrown",
                        "DarkKhaki",
                        "Silver",
                        "MediumVioletRed",
                        "IndianRed",
                        "Peru",
                        "Chocolate",
                        "Tan",
                        "LightGray",
                        "Thistle",
                        "Orchid",
                        "GoldenRod",
                        "PaleVioletRed",
                        "Crimson",
                        "Gainsboro",
                        "Plum",
                        "BurlyWood",
                        "LightCyan",
                        "Lavender",
                        "DarkSalmon",
                        "Violet",
                        "PaleGoldenRod",
                        "LightCoral",
                        "Khaki",
                        "AliceBlue",
                        "HoneyDew",
                        "Azure",
                        "SandyBrown",
                        "Wheat",
                        "Beige",
                        "WhiteSmoke",
                        "MintCream",
                        "GhostWhite",
                        "Salmon",
                        "AntiqueWhite",
                        "Linen",
                        "LightGoldenRodYellow",
                        "OldLace",
                        "Red",
                        "Fuchsia",
                        "Magenta",
                        "DeepPink",
                        "OrangeRed",
                        "Tomato",
                        "HotPink",
                        "Coral",
                        "Darkorange",
                        "LightSalmon",
                        "Orange",
                        "LightPink",
                        "Pink",
                        "Gold",
                        "PeachPuff",
                        "NavajoWhite",
                        "Moccasin",
                        "Bisque",
                        "MistyRose",
                        "BlanchedAlmond",
                        "PapayaWhip",
                        "LavenderBlush",
                        "SeaShell",
                        "Cornsilk",
                        "LemonChiffon",
                        "FloralWhite",
                        "Snow",
                        "Yellow",
                        "LightYellow",
                        "Ivory",
                        "White"
                    };
                }
                return _nameset;
            }
        }

        //public static List<ColorDeclaration> GetColorDeclarations(SiteDb SiteDb, Guid PageId=default(Guid))
        //{
        //    List<CmsCssDeclaration> AllDeclarations = null;
        //    List<CmsCssRule> AllRules = null;

        //    if (PageId == default(Guid))
        //    {
        //        AllRules = SiteDb.CssRules.All();
        //        AllDeclarations = SiteDb.CssDeclarations.All();
        //    }
        //    else
        //    {
        //        var allids = SiteDb.Pages.GetRelatedOwnerObjectIds(PageId);
        //        AllDeclarations = SiteDb.CssDeclarations.Query.WhereIn<Guid>(o => o.OwnerObjectId, allids).SelectAll();
        //        AllRules = SiteDb.CssRules.Query.WhereIn<Guid>(o => o.OwnerObjectId, allids).SelectAll();
        //    }

        //    Func<Guid, CmsCssRule> GetRule = (ruleid) =>
        //     {
        //         var find = AllRules.Find(o => o.Id == ruleid);
        //         if (find != null)
        //         {
        //             return find;
        //         }
        //         return SiteDb.CssRules.Get(ruleid);
        //     };

        //    List<ColorDeclaration> result = new List<ColorDeclaration>();

        //    foreach (var item in AllDeclarations)
        //    {
        //        if (Sites.Tag.Property.CanHaveColor(item.PropertyName))
        //        {
        //            var color = Sites.Service.ColorService.SearchColor(item.Value);
        //            if (!string.IsNullOrEmpty(color))
        //            {
        //                var rule = GetRule(item.CmsCssRuleId);
        //                if (rule != null)
        //                {
        //                    ColorDeclaration decl = new ColorDeclaration();
        //                    decl.Color = color;
        //                    decl.DeclarationId = item.Id;

        //                    decl.RuleId = item.CmsCssRuleId;
        //                    decl.Selector = rule.SelectorText;
        //                    decl.Property = item.PropertyName;
        //                    decl.Value = item.Value;
        //                    decl.StyleId = rule.ParentStyleId;
        //                    decl.IsInline = rule.IsInline;
        //                    decl.KoobooId = rule.KoobooId;
        //                    decl.OwnerConstType = rule.OwnerObjectConstType;
        //                    decl.OwnerObjectId = rule.OwnerObjectId;
        //                    result.Add(decl);
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}
    }
}