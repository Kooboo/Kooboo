//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Mail.ViewModel
{
    public class ComposeViewModel
    {
        public int? MessageId { get; set; }

        public string Subject { get; set; }

        public int From { get; set; }

        private List<string> _to;

        public List<string> To
        {
            get { return _to ?? (_to = new List<string>()); }
            set
            {
                _to = value;
            }
        }

        private List<string> _cc;

        public List<string> Cc
        {
            get { return _cc ?? (_cc = new List<string>()); }
            set
            {
                _cc = value;
            }
        }

        private List<string> _bcc;

        public List<string> Bcc
        {
            get { return _bcc ?? (_bcc = new List<string>()); }
            set
            {
                _bcc = value;
            }
        }

        private List<Models.Attachment> _attachments;

        public List<Models.Attachment> Attachments
        {
            get { return _attachments ?? (_attachments = new List<Models.Attachment>()); }
            set
            {
                _attachments = value;
            }
        }

        public string Html { get; set; }
    }
}