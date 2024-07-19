using System;
using System.Linq;
using MimeKit;
using MimeKit.Utils;


namespace Kooboo.Mail.Multipart
{
    public class HeaderListReader
    {
        public HeaderListReader(HeaderList headerList)
        {
            this.HeaderList = headerList;
        }

        private HeaderList HeaderList { get; set; }

        public DateTimeOffset Date
        {

            get
            {
                DateTimeOffset date = default(DateTimeOffset);
                var header = this.HeaderList.FirstOrDefault(o => o.Id == MimeKit.HeaderId.Date);
                if (header != null)
                {
                    byte[] rawValue = header.RawValue;
                    if (DateUtils.TryParse(rawValue, 0, rawValue.Length, out date))
                    {
                        return date;
                    }
                }

                return date;
            }
        }

        private string _subject;
        public string Subject
        {
            get
            {
                if (_subject == null)
                {
                    if (this.HeaderList.Contains(HeaderId.Subject))
                    {
                        _subject = this.HeaderList[HeaderId.Subject];
                    }
                }
                return _subject;
            }
        }


        private string _inReplyTo;

        public string InReplyTo
        {
            get
            {
                if (_inReplyTo == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.InReplyTo);

                    if (header != null)
                    {
                        byte[] rawValue = header.RawValue;
                        _inReplyTo = MimeUtils.EnumerateReferences(rawValue, 0, rawValue.Length).FirstOrDefault();
                    }
                }

                return _inReplyTo;
            }
        }

        private string _messageId;

        public string MessageId
        {
            get
            {
                if (_messageId == null)
                {

                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.MessageId);

                    if (header != null)
                    {
                        byte[] rawValue = header.RawValue;
                        _messageId = MimeUtils.ParseMessageId(rawValue, 0, rawValue.Length);
                    }
                }
                return _messageId;
            }
        }


        private InternetAddressList _from;
        public InternetAddressList From
        {
            get
            {
                if (_from == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.From);
                    var add = GetAddressList(header);
                    _from = add;
                }
                return _from;
            }
        }

        private InternetAddressList _ReplyTo;
        public InternetAddressList ReplyTo
        {
            get
            {
                if (_ReplyTo == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.ReplyTo);
                    var add = GetAddressList(header);
                    _ReplyTo = add;
                }
                return _ReplyTo;
            }
        }


        private InternetAddressList _To;
        public InternetAddressList To
        {
            get
            {
                if (_To == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.To);
                    var add = GetAddressList(header);
                    _To = add;
                }
                return _To;
            }
        }

        private InternetAddressList _Cc;
        public InternetAddressList Cc
        {
            get
            {
                if (_Cc == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.Cc);
                    var add = GetAddressList(header);
                    _Cc = add;
                }
                return _Cc;
            }
        }


        private InternetAddressList _Bcc;
        public InternetAddressList Bcc
        {
            get
            {
                if (_Bcc == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.Bcc);
                    var add = GetAddressList(header);
                    _Bcc = add;
                }
                return _Bcc;
            }
        }


        private MailboxAddress _sender;
        public MailboxAddress Sender
        {
            get
            {
                if (_sender == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.Sender);
                    var add = GetMailBox(header);
                    _sender = add;
                }
                return _sender;
            }
        }

        private InternetAddressList GetAddressList(Header header)
        {
            if (header == null)
            {
                return null;
            }

            if (InternetAddressList.TryParse(header.RawValue, out var adds))
            {
                return adds;
            }

            return null;
        }

        private MailboxAddress GetMailBox(Header header)
        {
            if (header == null)
            {
                return null;
            }

            if (MailboxAddress.TryParse(header.RawValue, out var adds))
            {
                return adds;
            }

            return null;
        }


        private ContentDisposition _ContentDisposition;
        public ContentDisposition ContentDisposition
        {
            get
            {
                if (_ContentDisposition == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.ContentDisposition);

                    if (header != null)
                    {
                        if (ContentDisposition.TryParse(header.RawValue, out var disposition))
                        {
                            _ContentDisposition = disposition;
                        }
                    }
                }

                return _ContentDisposition;
            }
        }


        private ContentType _contentType;

        public ContentType ContentType
        {
            get
            {
                if (_contentType == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.ContentType);

                    if (header != null)
                    {
                        if (ContentType.TryParse(header.RawValue, out var type))
                        {
                            _contentType = type;
                        }
                    }
                }

                return _contentType;
            }
        }


        private ContentEncoding _encoding;
        private bool EncodingCheck = false;

        public ContentEncoding ContentTransferEncoding
        {
            get
            {
                if (!EncodingCheck)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.ContentTransferEncoding);

                    if (header != null)
                    {
                        if (MimeUtils.TryParse(header.Value, out _encoding))
                        {

                        }
                    }

                    EncodingCheck = true;
                }

                return _encoding;
            }

        }

        private string _contentId;
        public string ContentId
        {
            get
            {
                if (_contentId == null)
                {
                    var header = this.HeaderList.FirstOrDefault(o => o.Id == HeaderId.ContentId);

                    if (header != null)
                    {
                        byte[] rawValue = header.RawValue;
                        _contentId = MimeUtils.ParseMessageId(rawValue, 0, rawValue.Length);
                    }
                }
                return _contentId;
            }
        }

    }


}
