//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Imap
{
    public class ImapResponse
    {
        public ImapResponse(string line)
        {
            this.Line = line;
        }

        public ImapResponse(byte[] bytes)
        {
            this.Binary = bytes;
        }

        public string Line { get; private set; }

        public byte[] Binary { get; private set; }
    }
}
