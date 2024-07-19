using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MimeKit;


namespace Kooboo.Mail.Multipart
{
    public class MultiPartHelper
    {

        public static List<LocationInfo> SplitParts(string MsgSource, int startPos, string boundary)
        {
            if (startPos < 0)
            {
                startPos = 0;
            }

            List<LocationInfo> result = new List<LocationInfo>();
            int totalLen = MsgSource.Length;

            string Begin = "--" + boundary;

            string End = "--" + boundary + "--";

            var index = MsgSource.IndexOf(Begin, startPos);

            LocationInfo info = null;

            bool HasInfo = false;

            while (index > -1)
            {
                var endIndex = index + Begin.Length;

                var nextOne = GetChar(endIndex + 1);
                var nextTwo = GetChar(endIndex + 2);

                if (nextOne == '-' && nextTwo == '-')
                {
                    if (info != null)
                    {
                        var previousOne = GetChar(index - 1);
                        var previousTwo = GetChar(index - 2);

                        if (previousOne == 10 || previousOne == 13)
                        {
                            index -= 1;
                        }

                        if (previousTwo == 10 || previousTwo == 13)
                        {
                            index -= 1;
                        }
                        info.EndPos = index;
                        result.Add(info);
                    }
                    break;
                }

                if (!HasInfo)
                {
                    info = new LocationInfo();
                    if (nextOne == 10 || nextOne == 13)
                    {
                        endIndex += 1;
                    }

                    if (nextTwo == 10 || nextTwo == 13)
                    {
                        endIndex += 1;
                    }

                    info = new LocationInfo();
                    info.StartPos = endIndex + 1;
                    HasInfo = true;
                }
                else
                {
                    // close one and start another one. 
                    var previousOne = GetChar(index - 1);
                    var previousTwo = GetChar(index - 2);

                    if (previousOne == 10 || previousOne == 13)
                    {
                        index -= 1;
                    }

                    if (previousTwo == 10 || previousTwo == 13)
                    {
                        index -= 1;
                    }

                    info.EndPos = index;
                    result.Add(info);

                    info = new LocationInfo();

                    if (nextOne == 10 || nextOne == 13)
                    {
                        endIndex += 1;
                    }

                    if (nextTwo == 10 || nextTwo == 13)
                    {
                        endIndex += 1;
                    }

                    info = new LocationInfo();
                    info.StartPos = endIndex + 1;
                }

                index = MsgSource.IndexOf(Begin, endIndex);
            }

            return result;

            char GetChar(int nextIndex)
            {
                if (nextIndex >= totalLen)
                {
                    return (Char)0x1a;
                }
                return MsgSource[nextIndex];
            }


        }

        public static string ExtractBoundary(HeaderList headerList)
        {
            if (headerList == null)
            {
                return null;
            }

            var contentType = headerList.FirstOrDefault(o => o.Id == MimeKit.HeaderId.ContentType);

            if (contentType != null)
            {
                return ExtractBoundary(contentType.Value);
            }
            return null;
        }

        public static string ExtractBoundary(string contentType)
        {
            BoundaryProvider provider = new BoundaryProvider();
            return provider.ExtractBoundary(contentType);
        }

        public static int FindDoubleLineIndex(string MessageSource, int StartPos, int EndPos = 0)
        {
            if (StartPos > EndPos)
            {
                return -1;
            }

            bool ISEOF = false;

            if (MessageSource == null)
            {
                return -1;
            }

            if (EndPos == 0)
            {
                EndPos = MessageSource.Length;
            }

            var current = GetCurrent();
            while (current != (Char)0x1a)
            {
                if (current == 13)    // 13  \r
                {
                    var one = ReadAhead(1);
                    var two = ReadAhead(2);
                    var three = ReadAhead(3);

                    if (one == 10 && two == 13 && three == 10)
                    {
                        return StartPos;
                    }

                    if (two == (Char)0x1a || three == (Char)0x1a)
                    {
                        return StartPos;
                    }
                }
                else if (current == 10)    // 10  \n
                {
                    var ahead = ReadAhead(1);

                    if (ahead == 10 || ahead == (Char)0x1a)
                    {
                        return StartPos;
                    }
                }

                StartPos += 1;
                current = GetCurrent();
            }

            if (ISEOF)
            {
                return -1;
            }
            else
            {
                StartPos += 1;

                if (StartPos < EndPos)
                {
                    return FindDoubleLineIndex(MessageSource, StartPos, EndPos);
                }
                else
                {
                    return -1;
                }
            }


            char GetCurrent()
            {
                return GetChar(StartPos);
            }

            char ReadAhead(int count)
            {
                var index = StartPos + count;
                return GetChar(index);
            }

            char GetChar(int currentIndex)
            {
                if (currentIndex >= EndPos)
                {
                    return (Char)0x1a;
                }
                return MessageSource[currentIndex];
            }


        }

        public static string ReadHeaderPart(string MsgFileName)
        {
            if (System.IO.File.Exists(MsgFileName))
            {
                using (var reader = new StreamReader(MsgFileName))
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        line = reader.ReadLine();
                    }

                    StringBuilder result = new StringBuilder();

                    bool FirstLine = true;

                    while (!string.IsNullOrWhiteSpace(line))
                    {
                        if (FirstLine)
                        {
                            FirstLine = false;
                        }
                        else
                        {
                            result.Append("\r\n");
                        }

                        result.Append(line);

                        line = reader.ReadLine();
                    }

                    return result.ToString();
                }
            }

            return null;
        }


        public static HeaderList ParseHeaderList(string HeaderText)
        {
            System.IO.MemoryStream mo = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(HeaderText));

            var parser = new MimeParser(mo);

            try
            {
                return MimeKit.HeaderList.Load(mo);

            }
            catch (Exception)
            {

            }
            return null;
        }


        public static MimeEntity ParseEntity(string PartBody)
        {
            MemoryStream mo = new MemoryStream(Encoding.UTF8.GetBytes(PartBody));
            var parser = new MimeParser(mo);
            return parser.ParseEntity();
        }

        public static string GetBodyString(string msgBody, int DoubleLineStart, int EndPos)
        {
            var startPos = DoubleLineStart;
            var totalLen = msgBody.Length;
            var NewPos = startPos;
            // skip the next new line. \r\n\r\n
            for (int i = 1; i < 5; i++)
            {
                NewPos = DoubleLineStart + i;
                if (NewPos < totalLen)
                {
                    var currentChar = msgBody[NewPos];
                    if (currentChar == 10 || currentChar == 13)
                    {
                        startPos = NewPos;
                    }
                    else
                    {
                        startPos = NewPos;
                        break;
                    }
                }
            }

            if (EndPos == 0)
            {
                return msgBody.Substring(startPos);
            }
            else
            {
                if (startPos >= EndPos)
                {
                    return null;
                }
                else
                {
                    return msgBody.Substring(startPos, EndPos - startPos);
                }
            }
        }
    }



    public class BoundaryProvider
    {
        private string BoundaryMark = "boundary";
        public string ExtractBoundary(string ContentType)
        {
            if (ContentType == null)
            {
                return null;
            }

            int start = 0;
            var index = ContentType.IndexOf(BoundaryMark, start);

            while (index > -1)
            {
                var boundary = ConsumeBoundary(ContentType, index);

                if (!string.IsNullOrWhiteSpace(boundary))
                {
                    return boundary;
                }

                start = index + 1;
                index = ContentType.IndexOf(BoundaryMark, start);
            }

            return null;
        }

        private string ConsumeBoundary(string input, int Index)
        {
            var ReadIndex = Index + BoundaryMark.Length;

            var nextChar = readNonSpaceChar();

            if (nextChar == '=')
            {
                var StartChar = readNonSpaceChar();

                if (StartChar == '\'' || StartChar == '"')
                {
                    return ConsumeTill(StartChar);
                }
                else
                {
                    return ConsumeTillEnd(StartChar);
                }
            }
            else
            {
                return null;
            }

            string ConsumeTillEnd(char startChar)
            {
                string result = null;
                result += startChar;

                var readChar = ConsumeChar();
                while (readChar != (Char)0x1a && !char.IsWhiteSpace(readChar) && readChar != ';' && readChar != ',')
                {
                    result += readChar;
                    readChar = ConsumeChar();
                }
                return result;
            }

            string ConsumeTill(char endChar)
            {
                string result = null;

                var readChar = ConsumeChar();
                while (readChar != (Char)0x1a && readChar != endChar)
                {
                    result += readChar;
                    readChar = ConsumeChar();
                }
                return result;
            }


            char readNonSpaceChar()
            {
                var current = ConsumeChar();
                while (char.IsWhiteSpace(current))
                {
                    current = ConsumeChar();
                }
                return current;
            }

            char ConsumeChar()
            {
                if (ReadIndex >= input.Length)
                {
                    return (Char)0x1a;
                }
                else
                {
                    var read = input[ReadIndex];
                    ReadIndex += 1;
                    return read;
                }
            }
        }

    }





}
