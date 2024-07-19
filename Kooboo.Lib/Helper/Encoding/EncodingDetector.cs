//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Text;
using Kooboo.Lib.Helper.EncodingHelper;

namespace Kooboo.Lib.Helper
{

    public class EncodingDetector
    {
        public static Encoding GetEncoding(ref byte[] databytes, string ContentType = null)
        {
            Encoding encoding = null;
            if (!string.IsNullOrEmpty(ContentType))
            {
                var charset = Kooboo.Lib.Helper.W3Encoding.ExtractCharset(ContentType);
                if (!string.IsNullOrEmpty(charset))
                {
                    if (charset.ToLower() == "utf8")
                    {
                        charset = "utf-8";
                    }

                    try
                    {
                        encoding = System.Text.Encoding.GetEncoding(charset);
                        if (encoding != null)
                        {
                            return encoding;
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
            }

            encoding = W3Encoding.PreScanEncoding(databytes);
            if (encoding != null)
            {
                return encoding;
            }

            Ude.CharsetDetector detector = new Ude.CharsetDetector();
            detector.Feed(databytes, 0, databytes.Length);
            detector.DataEnd();

            if (!string.IsNullOrWhiteSpace(detector.Charset))
            {
                encoding = System.Text.Encoding.GetEncoding(detector.Charset);
            }

            if (encoding == null)
            {
                encoding = System.Text.Encoding.GetEncoding(W3Encoding.SystemDefaultEncoding);
            }

            return encoding;
        }

        // this only return ascii or utf8, only for the email usage now. 
        public static string GetTextCharset(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            Ude.CharsetDetector detector = new Ude.CharsetDetector();
            detector.Feed(bytes, 0, bytes.Length);
            detector.DataEnd();
            return detector.Charset;
        }

        public static EmailEncodingResult GetEmailEncoding(byte[] databytes)
        {
            var result = EncodingHelper.EmailEncoding.ScanCharSet(databytes);

            if (result != null)
            {
                return result;
            }

            Ude.CharsetDetector detector = new Ude.CharsetDetector();
            detector.Feed(databytes, 0, databytes.Length);
            detector.DataEnd();


            if (!string.IsNullOrWhiteSpace(detector.Charset))
            {
                if (result == null)
                {
                    result = new EmailEncodingResult();
                }

                result.Charset = detector.Charset;
                return result;
            }

            return null;

        }


        public static bool IsValidEncoding(string EncodingName)
        {
            return System.Text.Encoding.GetEncoding(EncodingName) != null;
        }

    }
}
