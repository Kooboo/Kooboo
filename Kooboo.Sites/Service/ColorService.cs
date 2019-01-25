//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.ViewModel;
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

            List<char> _buffer = new List<char>();

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
                    _buffer.Add(currentchar);
                }
                else
                {
                    string output = new string(_buffer.ToArray());
                    _buffer.Clear();

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
                    _nameset = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _nameset.Add("Black");
                    _nameset.Add("Navy");
                    _nameset.Add("DarkBlue");
                    _nameset.Add("MediumBlue");
                    _nameset.Add("Blue");
                    _nameset.Add("DarkGreen");
                    _nameset.Add("Green");
                    _nameset.Add("Teal");
                    _nameset.Add("DarkCyan");
                    _nameset.Add("DeepSkyBlue");
                    _nameset.Add("DarkTurquoise");
                    _nameset.Add("MediumSpringGreen");
                    _nameset.Add("Lime");
                    _nameset.Add("SpringGreen");
                    _nameset.Add("Aqua");
                    _nameset.Add("Cyan");
                    _nameset.Add("MidnightBlue");
                    _nameset.Add("DodgerBlue");
                    _nameset.Add("LightSeaGreen");
                    _nameset.Add("ForestGreen");
                    _nameset.Add("SeaGreen");
                    _nameset.Add("DarkSlateGray");
                    _nameset.Add("LimeGreen");
                    _nameset.Add("MediumSeaGreen");
                    _nameset.Add("Turquoise");
                    _nameset.Add("RoyalBlue");
                    _nameset.Add("SteelBlue");
                    _nameset.Add("DarkSlateBlue");
                    _nameset.Add("MediumTurquoise");
                    _nameset.Add("Indigo");
                    _nameset.Add("DarkOliveGreen");
                    _nameset.Add("CadetBlue");
                    _nameset.Add("CornflowerBlue");
                    _nameset.Add("MediumAquaMarine");
                    _nameset.Add("DimGray");
                    _nameset.Add("DimGrey");
                    _nameset.Add("SlateBlue");
                    _nameset.Add("OliveDrab");
                    _nameset.Add("SlateGray");
                    _nameset.Add("LightSlateGray");
                    _nameset.Add("MediumSlateBlue");
                    _nameset.Add("LawnGreen");
                    _nameset.Add("Chartreuse");
                    _nameset.Add("Aquamarine");
                    _nameset.Add("Maroon");
                    _nameset.Add("Purple");
                    _nameset.Add("Olive");
                    _nameset.Add("Gray");
                    _nameset.Add("SkyBlue");
                    _nameset.Add("LightSkyBlue");
                    _nameset.Add("BlueViolet");
                    _nameset.Add("DarkRed");
                    _nameset.Add("DarkMagenta");
                    _nameset.Add("SaddleBrown");
                    _nameset.Add("DarkSeaGreen");
                    _nameset.Add("LightGreen");
                    _nameset.Add("MediumPurple");
                    _nameset.Add("DarkViolet");
                    _nameset.Add("PaleGreen");
                    _nameset.Add("DarkOrchid");
                    _nameset.Add("YellowGreen");
                    _nameset.Add("Sienna");
                    _nameset.Add("Brown");
                    _nameset.Add("DarkGray");
                    _nameset.Add("LightBlue");
                    _nameset.Add("GreenYellow");
                    _nameset.Add("PaleTurquoise");
                    _nameset.Add("LightSteelBlue");
                    _nameset.Add("PowderBlue");
                    _nameset.Add("FireBrick");
                    _nameset.Add("DarkGoldenRod");
                    _nameset.Add("MediumOrchid");
                    _nameset.Add("RosyBrown");
                    _nameset.Add("DarkKhaki");
                    _nameset.Add("Silver");
                    _nameset.Add("MediumVioletRed");
                    _nameset.Add("IndianRed");
                    _nameset.Add("Peru");
                    _nameset.Add("Chocolate");
                    _nameset.Add("Tan");
                    _nameset.Add("LightGray");
                    _nameset.Add("Thistle");
                    _nameset.Add("Orchid");
                    _nameset.Add("GoldenRod");
                    _nameset.Add("PaleVioletRed");
                    _nameset.Add("Crimson");
                    _nameset.Add("Gainsboro");
                    _nameset.Add("Plum");
                    _nameset.Add("BurlyWood");
                    _nameset.Add("LightCyan");
                    _nameset.Add("Lavender");
                    _nameset.Add("DarkSalmon");
                    _nameset.Add("Violet");
                    _nameset.Add("PaleGoldenRod");
                    _nameset.Add("LightCoral");
                    _nameset.Add("Khaki");
                    _nameset.Add("AliceBlue");
                    _nameset.Add("HoneyDew");
                    _nameset.Add("Azure");
                    _nameset.Add("SandyBrown");
                    _nameset.Add("Wheat");
                    _nameset.Add("Beige");
                    _nameset.Add("WhiteSmoke");
                    _nameset.Add("MintCream");
                    _nameset.Add("GhostWhite");
                    _nameset.Add("Salmon");
                    _nameset.Add("AntiqueWhite");
                    _nameset.Add("Linen");
                    _nameset.Add("LightGoldenRodYellow");
                    _nameset.Add("OldLace");
                    _nameset.Add("Red");
                    _nameset.Add("Fuchsia");
                    _nameset.Add("Magenta");
                    _nameset.Add("DeepPink");
                    _nameset.Add("OrangeRed");
                    _nameset.Add("Tomato");
                    _nameset.Add("HotPink");
                    _nameset.Add("Coral");
                    _nameset.Add("Darkorange");
                    _nameset.Add("LightSalmon");
                    _nameset.Add("Orange");
                    _nameset.Add("LightPink");
                    _nameset.Add("Pink");
                    _nameset.Add("Gold");
                    _nameset.Add("PeachPuff");
                    _nameset.Add("NavajoWhite");
                    _nameset.Add("Moccasin");
                    _nameset.Add("Bisque");
                    _nameset.Add("MistyRose");
                    _nameset.Add("BlanchedAlmond");
                    _nameset.Add("PapayaWhip");
                    _nameset.Add("LavenderBlush");
                    _nameset.Add("SeaShell");
                    _nameset.Add("Cornsilk");
                    _nameset.Add("LemonChiffon");
                    _nameset.Add("FloralWhite");
                    _nameset.Add("Snow");
                    _nameset.Add("Yellow");
                    _nameset.Add("LightYellow");
                    _nameset.Add("Ivory");
                    _nameset.Add("White");
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
