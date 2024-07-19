//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

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

        public static string ToBase64Image(string mineType, string Base64String)
        {
            //string uri = @"<img src=""data:image/png;base64,iVBORw0KGgoAAA
            //ANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4
            ////8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU
            //5ErkJggg=="" alt=""Red dot"" />";

            string output = "data:" + mineType + ";base64," + Base64String;
            return output;
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


    // If <MIME-type> is omitted, it defaults to text/plain;charset=US-ASCII.
    //data:[<MIME-type>][;charset=<encoding>][;base64],<data>
    //<data> is a sequence of octets. If ;base64 is present, the data is encoded as base64. Otherwise, the data is represented using percent-encoding, using ASCII for octets inside the range of safe URL characters and %xx hex encoding for octets outside that range.

    public class DataUriSchema
    {
        private string _minetype;

        public string MineType
        {
            get
            {
                if (string.IsNullOrEmpty(_minetype))
                {
                    return "text/plain";
                }
                else
                {
                    return _minetype;
                }
            }
            set
            {
                _minetype = value;
            }
        }

        private string _charset;
        public string CharSet
        {
            get
            {
                if (string.IsNullOrEmpty(_charset) && string.IsNullOrEmpty(_minetype))
                {
                    return "US-ASCII";
                }
                else
                {
                    return _charset;
                }
            }
            set
            {
                _charset = value;
            }
        }

        public string DataString { get; set; }

        public bool isBase64 { get; set; }

    }


}
