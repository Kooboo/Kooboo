//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using LumiSoft.Net.MIME;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Utility
{
    public static class HeaderUtility
    {

        public static string EncodeField(string input, bool IsAddress = false)
        {
            if (ShouldEncode(input))
            {
                var wordEncoder = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8);

                if (IsAddress)
                {
                    int index = input.IndexOf("<");
                    int endindex = input.LastIndexOf(">"); 
                    if (index>0 && endindex > index)
                    {
                        var word = input.Substring(0, index);
                        var encoded = wordEncoder.Encode(word);
                        return encoded + input.Substring(index); 
                    }
                } 
                return wordEncoder.Encode(input);
               
            }
            return input; 
        }

        public static string DecodeField(string encodedString)
        {
            var wordEncoder = new LumiSoft.Net.MIME.MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8);
            return wordEncoder.Decode(encodedString);
        }

        private static bool ShouldEncode(string input)
        {
            if (input == null)
            {
                return false; 
            }
            int len = input.Length;

            for (int i = 0; i < len; i++)
            {
                if (input[i] > 127)
                {
                    return true; 
                }
            } 
            return false; 
        }
        
        public static string RepalceRepyTo(string header, string newReplyTo)
        {
            string field = "reply-to";

            var index = header.IndexOf(field, StringComparison.OrdinalIgnoreCase); 

            if (index ==-1)
            {
                return "Reply-To:" + newReplyTo + "\r\n" + header; 
            } 
            else
            {
               if (index == 0)
                {
                    // the start. 
                    int nextline = header.IndexOf("\r\n"); 
                    return "Reply-To:" + newReplyTo + header.Substring(nextline);  
                }
               else
                {
                    string startfield = "\r\nReply-To"; 
                    int start = header.IndexOf(startfield, StringComparison.OrdinalIgnoreCase);
                    if (start > 0)
                    {
                        string before = header.Substring(0, start);

                        int endindex = header.IndexOf("\r\n", start + startfield.Length);

                        string end = header.Substring(endindex + 2);

                        return before + "\r\nReply-To:" + newReplyTo + "\r\n" + end;  
                    }
                }
            }

            return null; 
        }
         

    }
}
