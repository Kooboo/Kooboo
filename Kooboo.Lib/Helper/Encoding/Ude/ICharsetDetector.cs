//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;

namespace Ude
{
    public interface ICharsetDetector
    {
        string Charset
        {
            get;
        }

        float Confidence
        {
            get;
        }

        void Feed(byte[] buf, int offset, int len);

        void Feed(Stream stream);

        void Reset();

        bool IsDone();

        void DataEnd();
    }
}
