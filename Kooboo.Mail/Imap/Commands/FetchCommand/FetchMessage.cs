//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Text;
using Kooboo.Mail.Multipart;
using MimeKit;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    /// <summary>
    /// Use this class to lazy load content and parse content to avoid reload action in multiple FETCH data item process
    /// </summary>
    public class FetchMessage : IDisposable
    {
        public MailDb MailDb { get; set; }

        public Message Message { get; set; }

        public int SeqNo { get; set; }


        internal string _tempFileName;

        internal string GetMsgFileName()
        {
            if (_tempFileName != null)
            {
                return _tempFileName;
            }

            if (this.MailDb != null && this.Message != null)
            {
                return this.MailDb.MsgHandler.GetMsgFileName(this.Message.MsgId, this.Message.BodyPosition);
            }
            return null;
        }

        public string GetTextSource()
        {
            var fileName = GetMsgFileName();
            if (fileName != null && System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllText(fileName);
            }

            if (!string.IsNullOrEmpty(this.Message.Body))
            {
                return this.Message.Body;
            }
            return null;
        }

        public long GetByteLength()
        {
            var fileName = GetMsgFileName();
            if (fileName != null && System.IO.File.Exists(fileName))
            {
                var info = new System.IO.FileInfo(fileName);
                return info.Length;
            }

            if (this.Message != null && !string.IsNullOrEmpty(this.Message.Body))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(this.Message.Body);
                return bytes.Length;
            }

            return 0;
        }

        public byte[] GetBytes()
        {
            var fileName = GetMsgFileName();
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);
            }
            if (this.Message != null && !string.IsNullOrEmpty(this.Message.Body))
            {
                return System.Text.Encoding.UTF8.GetBytes(this.Message.Body);
            }

            return null;
        }

        private HeaderList _mailHeader;
        public HeaderList MailHeader
        {
            get
            {
                if (_mailHeader is null)
                {
                    var fileName = GetMsgFileName();
                    if (fileName != null && System.IO.File.Exists(fileName))
                    {
                        string HeaderText = MultiPartHelper.ReadHeaderPart(fileName);
                        var list = MultiPartHelper.ParseHeaderList(HeaderText);
                        _mailHeader = list;
                    }
                }

                return _mailHeader;
            }
            set
            {
                _mailHeader = value;
            }
        }

        private MultiPartInfo _partInfo;
        public MultiPartInfo PartInfo
        {
            get
            {
                if (_partInfo == null)
                {
                    var fileName = GetMsgFileName();
                    if (fileName != null && System.IO.File.Exists(fileName))
                    {
                        var MsgBody = System.IO.File.ReadAllText(fileName);
                        var part = new MultiPartInfo(MsgBody);
                        _partInfo = part;
                    }
                }
                return _partInfo;
            }
            set
            {
                _partInfo = value;
            }
        }


        private MimeMessage _mailMessage;
        public MimeMessage MailMessage
        {
            get
            {
                if (_mailMessage is null)
                {
                    var path = Path.Combine(MailDb.MsgFolder, Message.MsgId.ToString() + ".eml");
                    if (File.Exists(path))
                    {
                        var fileStream = new FileStream(path, FileMode.Open);
                        _mailMessage = MimeMessage.Load(fileStream);
                        fileStream.Close();
                    }
                    else
                    {
                        var body = MailDb.MsgHandler.GetMsgBody(Message.MsgId, Message.BodyPosition);
                        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
                        var mailParser = new ExperimentalMimeParser(memoryStream);
                        _mailMessage = mailParser.ParseMessage();
                    }
                }

                return _mailMessage;
            }
            set
            {
                _mailMessage = value;
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MailDb.Dispose();
                }

                if (_mailMessage is not null)
                    _mailMessage.Dispose();
                Message = null;
                if (_mailHeader is not null)
                    _mailHeader = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        //private MimeMessage _mailkitmsg;

        //public MimeMessage MailkitMsg
        //{
        //    get
        //    {
        //        if (_mailkitmsg == null)
        //        {
        //            var body = MailDb.MsgHandler.GetMsgBody(Message.MsgId, Message.BodyPosition);   //MailDb.MsgBody2.Get(Message.BodyPosition);
        //            _mailkitmsg = Kooboo.Mail.Utility.MailKitUtility.LoadMessage(body);
        //        }
        //        return _mailkitmsg;
        //    }
        //    set { _mailkitmsg = value;}
        //}
    }
}
