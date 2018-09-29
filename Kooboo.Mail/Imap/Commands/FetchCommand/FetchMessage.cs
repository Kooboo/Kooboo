//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LumiSoft.Net.Mail;

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
            get
            {
                if (_content == null)
                {
                    _content = MailDb.Messages.GetContent(Message.Id);
                }
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        private byte[] _bytes;
        public byte[] Bytes
        {
            get
            {
                if (_bytes == null)
                {
                    _bytes = Encoding.UTF8.GetBytes(Content);
                }
                return _bytes;
            }
        }

        private Mail_Message _parsed;
        public Mail_Message Parsed
        {
            get
            {
                if (_parsed == null)
                {
                    _parsed = Mail_Message.ParseFromByte(Bytes);
                }
                return _parsed;
            }
        }
    }
}
