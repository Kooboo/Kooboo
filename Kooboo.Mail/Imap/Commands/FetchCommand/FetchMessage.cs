//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using LumiSoft.Net.Mail;
using System.Text;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    /// <summary>
    /// Use this class to lazy load content and parse content to avoid reload action in multiple FETCH data item process
    /// </summary>
    public class FetchMessage
    {
        public MailDb MailDb { get; set; }

        public Message Message { get; set; }

        public int SeqNo { get; set; }

        private string _content;

        public string Content
        {
            get { return _content ?? (_content = MailDb.Messages.GetContent(Message.Id)); }
            set
            {
                _content = value;
            }
        }

        private byte[] _bytes;

        public byte[] Bytes
        {
            get { return _bytes ?? (_bytes = Encoding.UTF8.GetBytes(Content)); }
        }

        private Mail_Message _parsed;

        public Mail_Message Parsed
        {
            get { return _parsed ?? (_parsed = Mail_Message.ParseFromByte(Bytes)); }
        }
    }
}