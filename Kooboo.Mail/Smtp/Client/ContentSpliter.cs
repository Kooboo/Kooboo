using System.Collections.Generic;

namespace Kooboo.Mail.Smtp.Client
{
    public static class ContentSplitter
    {
        public static List<DataBlock> Split(string OriginalText, int bufferSize = 4096)
        {
            List<DataBlock> Result = new List<DataBlock>();

            int totalLen = OriginalText.Length;

            int okSize = bufferSize / 2;
            var index = OriginalText.IndexOf("\r\n");
            int currentRead = 0;
            int TempRead = 0;

            while (index > -1)
            {

                if ((index - currentRead) > bufferSize)
                {
                    // too much. 
                    DataBlock block = new DataBlock() { Start = currentRead, End = TempRead };
                    currentRead = TempRead;
                    Result.Add(block);
                }

                else if ((index - currentRead) > okSize)
                {
                    DataBlock block = new DataBlock() { Start = currentRead, End = index + 2 };
                    currentRead = index + 2;
                    TempRead = currentRead;
                    Result.Add(block);
                }

                else
                {
                    TempRead = index + 2;
                }

                if (TempRead < totalLen)
                {
                    index = OriginalText.IndexOf("\r\n", TempRead);
                }
                else
                {
                    index = -1;
                }

            }

            if (currentRead < totalLen - 1)
            {
                DataBlock block = new DataBlock() { Start = currentRead, End = totalLen };
                Result.Add(block);
            }

            return Result;
        }

        public static string ReadBlock(string OriginalText, DataBlock block)
        {
            return OriginalText.Substring(block.Start, block.End - block.Start);
        }
    }

    public struct DataBlock
    {
        public int Start;
        public int End;
    }
}


