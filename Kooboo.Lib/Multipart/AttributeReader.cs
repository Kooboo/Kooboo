//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.NETMultiplePart
{

    public class AttributeReader
    {
        public static Dictionary<string, string> GetAttributes(string input)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string[] segs = input.Split(';');

            foreach (var item in segs)
            {
                var attributes = GetAttribute(item);
                if (attributes != null && attributes.Count() > 0)
                {
                    foreach (var dict in attributes)
                    {
                        if (!result.ContainsKey(dict.Key))
                        {
                            result.Add(dict.Key, dict.Value);
                        }
                    }
                }
            }

            return result;
        }

        public static Dictionary<string, string> GetAttribute(string input)
        {
            Dictionary<string, string> returnDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string name = string.Empty;
            string value = string.Empty;
            var state = AttributeState.beforeName;

            char quote = default(char);
            bool hasquote = false;

            int index = 0;
            int len = input.Length;

            while (index < len)
            {
                var current = input[index];
                if (state == AttributeState.beforeName)
                {
                    if (!NonData(current))
                    {
                        name += current;
                        state = AttributeState.InName;
                    }
                }
                else if (state == AttributeState.InName)
                {
                    if (NonData(current))
                    {
                        state = AttributeState.AfterName;
                        index--;
                        if (index < 0)
                        {
                            index = 0;
                        }
                    }
                    else
                    {
                        name += current;
                    }
                }
                else if (state == AttributeState.AfterName)
                {
                    if (current == '=')
                    {
                        state = AttributeState.BeforeValue;
                    }
                    else
                    {
                        if (!NonData(current))
                        {
                            returnDict.Add(name, "");
                            name = null;
                            value = null;
                            hasquote = false;
                            state = AttributeState.beforeName;
                        }
                    }
                }
                else if (state == AttributeState.BeforeValue)
                {
                    if (!NonData(current))
                    {
                        if (current == '\'' || current == '\"')
                        {
                            hasquote = true;
                            quote = current;
                            state = AttributeState.InValue;
                        }
                        else
                        {
                            value += current;
                            state = AttributeState.InValue;
                        }
                    }
                }
                else if (state == AttributeState.InValue)
                {
                    if (hasquote)
                    {
                        if (current == quote)
                        {
                            returnDict.Add(name, value);
                            name = null;
                            value = null;
                            hasquote = false;
                            state = AttributeState.beforeName;
                        }
                        else
                        {
                            value += current;
                        }
                    }
                    else
                    {
                        if (NonData(current))
                        {
                            returnDict.Add(name, value);
                            name = null;
                            value = null;
                            hasquote = false;
                            state = AttributeState.beforeName;
                        }
                        else
                        {
                            value += current;
                        }
                    }

                }
                else
                {
                    break;
                }

                index += 1;
            }

            return returnDict;
        }

        private static bool NonData(char current)
        {
            if (Kooboo.Lib.Helper.CharHelper.isSpaceCharacters(current) || current == '<' || current == '=' || current == '>')
            {
                return true;
            }
            return false;
        }
    }

    public enum AttributeState
    {
        beforeName = 0,
        InName = 1,
        AfterName = 2,
        BeforeValue = 3,
        InValue = 4
    }

}
