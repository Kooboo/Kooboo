//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Scripting
{
    public static class ScriptHelper
    { 
        public static List<string> GetkScriptParaFromDom(SiteDb sitedb, Kooboo.Dom.Document doc)
        {
            HashSet<string> paras = new HashSet<string>();

            if (doc == null)
            {
                return paras.ToList(); 
            }

            var scripts = doc.getElementsByTagName("script");

            foreach (var item in scripts.item)
            {
                if (Kooboo.Sites.Render.Components.Manager.IsComponent(item))
                {
                    if (!string.IsNullOrEmpty(item.id))
                    {
                        var code = sitedb.Code.Get(item.id);
                        if (code != null && code.Parameters !=null)
                        {
                            foreach (var p in code.Parameters)
                            {
                                if (!string.IsNullOrWhiteSpace(p))
                                {
                                    paras.Add(p);
                                } 
                            }
                        }
                    }

                    // script component, only kscript now. 
                    else if (!string.IsNullOrEmpty(item.InnerHtml))
                    {
                        var scriptparas = GetKScriptParameters(item.InnerHtml);
                        if (scriptparas != null)
                        {
                            foreach (var p in scriptparas)
                            {
                                if (!string.IsNullOrEmpty(p))
                                {
                                    paras.Add(p);
                                }
                            }
                        }
                    }
                }
            }

            return paras.ToList();
        }

        public static List<string> GetKScriptParameters(string source)
        {
            List<string> paras = new List<string>();

            if (string.IsNullOrWhiteSpace(source))
            {
                return paras; 
            }

            int len = source.Length;

            Func<int, char> GetChar = (index) =>
            {
                if (index >= len || index < 0)
                {
                    return default(char);
                }
                return source[index];
            };


            int position = 0;

            while (position < len)
            {
                var current = GetChar(position);

                if (current == '\'' || current == '\"')
                {
                    int nextindex = position + 1;
                    var nextchar = GetChar(nextindex);

                    while (true)
                    {
                        if (nextchar == '\\')
                        {
                            nextindex += 2;
                            nextchar = GetChar(nextindex);
                            continue;
                        }

                        if (nextchar == current)
                        {
                            position = nextindex + 1;
                            break;
                        }

                        if (nextchar == default(char))
                        {
                            position = nextindex + 1;
                            break;
                        }
                        nextindex += 1;
                        nextchar = GetChar(nextindex);
                    }
                    continue;
                }

                if (current == '/')
                {

                    var nextindex = position + 1;
                    var nextchar = GetChar(nextindex);
                    // for the // 
                    if (nextchar == '/')
                    {
                        // look till the next line. 
                        nextindex += 1;
                        nextchar = GetChar(nextindex);
                        while (nextchar != '\r' && nextchar != '\n' && nextchar != default(char))
                        {
                            nextindex += 1;
                            nextchar = GetChar(nextindex);
                        }

                        position = nextindex + 1;
                        continue;

                    }
                    else if (nextchar == '*')
                    {
                        // for the /*
                        // look till the next */. 
                        nextindex += 1;
                        nextchar = GetChar(nextindex);

                        while (nextchar != '*' && GetChar(nextindex + 1) != '/')
                        {
                            nextindex += 1;
                            nextchar = GetChar(nextindex);
                        }
                        position = nextindex + 2;
                        continue;
                    }

                }


                // k.request.
                if (GetChar(position) == 'k' && GetChar(position + 1) == '.' && GetChar(position + 2) == 'r' && GetChar(position + 3) == 'e' && GetChar(position + 4) == 'q' && GetChar(position + 5) == 'u' && GetChar(position + 6) == 'e' && GetChar(position + 7) == 's' && GetChar(position + 8) == 't')
                {
                    // and make sure that k is not part of something. 
                    var prechar = GetChar(position - 1);
                    if (!IsValidVarName(prechar))
                    {
                        int end = 0;
                        // get the para value. 
                        var key = GetParaKey(source, position + 8 + 1, len, ref end);

                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            key = "{" + key + "}"; 
                            paras.Add(key);

                            int nextpos = position + 8 + 1;
                            if (end > nextpos)
                            {
                                nextpos = end;
                            }
                            position = nextpos;
                        } 
                    } 
                }

                position += 1;
            }


            return paras;


        }

        private static bool IsValidVarName(char chr)
        {
            // The uppercase ASCII letters are the characters in the range uppercase ASCII letters. A-Z
            if (chr >= 65 && chr <= 90)
            {
                return true;
            }
            //a-z, ascii 97-122. 
            if (chr >= 97 && chr <= 122)
            {
                return true;
            }
            //0-9, acsii 48-57. 
            if (chr >= 48 && chr <= 57)
            {
                return true;
            }

            if (chr=='_')
            {
                return true; 
            }
            return false;
        }

        private static bool IsValidParaKey(string input)
        {
            int len = input.Length;
            for (int i = 0; i < len; i++)
            {
                if (!IsValidVarName(input[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetParaKey(string source, int pos, int Len, ref int end)
        {

            Func<int, char> GetChar = (index) =>
            {
                if (index >= Len || index < 0)
                {
                    return default(char);
                }
                return source[index];
            };

            // for .querystring, skip the query. 
            var currentChr = GetChar(pos);

            if (currentChr == '.' && GetChar(pos + 1) == 'q' && GetChar(pos + 2) == 'u' && GetChar(pos + 3) == 'e' && GetChar(pos + 4) == 'r' && GetChar(pos + 5) == 'y' && GetChar(pos + 6) == 'S' && GetChar(pos + 7) == 't' && GetChar(pos + 8) == 'r' && GetChar(pos + 9) == 'i' && GetChar(pos + 10) == 'n' && GetChar(pos + 11) == 'g')
            {
                var nextchar = GetChar(pos + 12);

                if (!IsValidVarName(nextchar))
                {
                    pos += 12;
                    currentChr = GetChar(pos);
                }
            }

            // for .form, skip... 
            if (currentChr == '.' && GetChar(pos + 1) == 'f' && GetChar(pos + 2) == 'o' && GetChar(pos + 3) == 'r' && GetChar(pos + 4) == 'm')
            {
                var next = GetChar(pos + 5);
                if (!IsValidVarName(next))
                {
                    return null;
                }
            }

            // if []
            if (currentChr == '[')
            {
                // read till the next ]; 
                string text = string.Empty;
                pos += 1;
                currentChr = GetChar(pos);

                while (currentChr != ']')
                {
                    if (currentChr == '\'' || currentChr == '"' || IsValidVarName(currentChr) || Lib.Helper.CharHelper.isSpaceCharacters(currentChr))
                    {
                        text += currentChr;
                    }
                    else
                    {
                        break;
                    }

                    pos += 1;
                    currentChr = GetChar(pos);
                }

                if (!string.IsNullOrEmpty(text))
                {
                    text = text.Trim();
                    text = text.Trim('\'');
                    text = text.Trim('"');
                    text = text.Trim();
                    // check to make sure it is a valid. 
                    if (IsValidParaKey(text))
                    {
                        end = pos;
                        return text;
                    }
                }
            }

            else if (currentChr == '.' && GetChar(pos + 1) == 'g' && GetChar(pos + 2) == 'e' && GetChar(pos + 3) == 't')
            {
                int nextindex = pos + 4;
                var nextchar = GetChar(nextindex);
                while (Lib.Helper.CharHelper.isSpaceCharacters(nextchar))
                {
                    nextindex += 1;
                    nextchar = GetChar(nextindex);
                }

                if (nextchar != '(')
                {
                    return null;
                }
                else
                {
                    // read till the next ]; 
                    string text = string.Empty;
                    nextindex += 1;
                    currentChr = GetChar(nextindex);

                    while (currentChr != ')')
                    {
                        if (currentChr == '\'' || currentChr == '"' || IsValidVarName(currentChr) || Lib.Helper.CharHelper.isSpaceCharacters(currentChr))
                        {
                            text += currentChr;
                        }
                        else
                        {
                            break;
                        }
                        nextindex += 1;
                        currentChr = GetChar(nextindex);
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        text = text.Trim();
                        text = text.Trim('\'');
                        text = text.Trim('"');
                        text = text.Trim();
                        // check to make sure it is a valid. 
                        if (IsValidParaKey(text))
                        {
                            end = nextindex;

                            return text;
                        }
                    }

                }
            }

            else if (currentChr == '.')
            {
                // the case of k.request.key; 
                string text = string.Empty;
                pos += 1;
                currentChr = GetChar(pos);

                while (currentChr != ';')
                {
                    if (IsValidVarName(currentChr))
                    {
                        text += currentChr;
                    }
                    else
                    {
                        break;
                    }
                    pos += 1;
                    currentChr = GetChar(pos);
                }

                if (!string.IsNullOrWhiteSpace(text))
                {
                    end = pos;
                    return text;
                }

            }

            return null;

        }

    }
}
