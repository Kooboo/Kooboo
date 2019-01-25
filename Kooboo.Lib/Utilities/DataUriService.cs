//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Lib.Utilities
{
    public class DataUriService
    {
        public static DataUriSchema PraseDataUri(string input)
        {
            DataUriSchema schema = new DataUriSchema();

            int length = input.Length;
            string temp = string.Empty;

            for (int i = 0; i < length; i++)
            {
                if (isSpaceCharacters(input[i]))
                {
                    continue;
                }

                if (input[i] == ':')
                {
                    if (temp.ToLower() != "data")
                    {
                        return null;
                    }
                    else
                    {
                        // the data: will be ignored. 
                        temp = string.Empty;
                    }
                    continue;
                }

                else if (input[i] == ',')
                {
                    // temp contains mine and charset, etc. 
                    string[] sets = temp.Split(';');

                    foreach (string item in sets)
                    {
                        string lower = item.ToLower();
                        if (lower.Contains("base64"))
                        {
                            schema.isBase64 = true;
                        }
                        else if (lower.Contains("charset"))
                        {
                            int equalindex = lower.IndexOf("=");
                            if (equalindex > 0 && equalindex < lower.Length - 1)
                            {
                                schema.CharSet = lower.Substring(equalindex + 1);
                            }
                        }
                        else
                        {
                            // this should be the mine now. To be checked. 
                            schema.MineType = lower;

                        }
                    }


                    // The rest of the string is the data. 
                    schema.DataString = input.Substring(i + 1);

                    return schema;

                }

                temp += input[i].ToString();
            }

            return schema;

        }


        public static bool isDataUri(string src)
        {
            if (String.IsNullOrWhiteSpace(src))
            {
                return false;
            }

            int length = src.Length;

            if (length < 10)
            {
                return false;
            }

            string startwith = string.Empty;
            int count = 0;

            for (int i = 0; i < length; i++)
            {
                if (isSpaceCharacters(src[i]) || src[i] == '/' || src[i] == '\\')
                {
                    continue;
                }

                startwith += src[i].ToString();

                count += 1;

                if (count >= 5)
                {
                    if (startwith.ToLower() == "data:")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private static bool isSpaceCharacters(char chr)
        {
            return (chr == '\u0020' || chr == '\u0009' || chr == '\u000a' || chr == '\u000c' || chr == '\u000d');
        }

    }
}
