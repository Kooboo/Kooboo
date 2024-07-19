//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail
{
    public class Message : IMailObject, IndexedDB.WORM.MetaObject.IMetaObject
    {
        public Message()
        {

        }

        public MailDb maildb { get; set; }

        public int Id { get; set; }    // this only used by old repository...which is gone now. or as the temp SEQ NO now. 

        public long MsgId { get; set; }  // New Id long for WORM.

        public string SmtpMessageId { get; set; }

        // User id is not needed, because every user has access to his own mail storage only. 
        public Guid UserId { get; set; }

        public int AddressId { get; set; }

        public bool OutGoing { get; set; }

        public int FolderId { get; set; }

        /// <summary>
        /// The SMTP Mail From. The envelope address.
        /// The return Address. 
        /// </summary>
        public string MailFrom { get; set; }

        public string RcptTo { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public long BodyPosition { get; set; }

        private string _body;
        public String Body
        {
            get
            {
                if (_body == null && this.maildb != null)
                {
                    _body = this.maildb.MsgHandler.GetMsgBody(this.MsgId, this.BodyPosition);

                }
                return _body;
            }
            set
            {
                _body = value;
            }
        }   // new body storage. 

        public string Summary { get; set; }

        public int Size { get; set; }

        public bool Read { get; set; }

        public bool Answered { get; set; }

        public bool Deleted { get; set; }

        public bool Flagged { get; set; }

        public bool Recent { get; set; } = true;


        private DateTime _creationtime;

        public DateTime CreationTime
        {
            get
            {
                if (_creationtime == default(DateTime))
                {
                    _creationtime = DateTime.UtcNow;
                }
                return _creationtime;
            }
            set
            {
                _creationtime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                //_creationTimetick = default(long);
            }
        }


        private long _creationTimetick;

        public long CreationTimeTick
        {
            get
            {
                if (_creationTimetick == default(long))
                {
                    _creationTimetick = this.CreationTime.Ticks;
                }
                return _creationTimetick;
            }
            set
            {
                _creationTimetick = value;
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public bool Draft
        {
            get
            {
                return this.FolderId == Folder.ToId("Draft");
            }
        }

        private List<Models.Attachment> _attachments;
        public List<Models.Attachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments = new List<Models.Attachment>();
                }
                return _attachments;
            }
            set
            {
                _attachments = value;
            }
        }

        public bool HasAttachment
        {
            get
            {
                if (_attachments != null && _attachments.Count() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public int InviteConfirm { get; set; }

        [Kooboo.Data.Attributes.KIgnore]
        public int MetaByteLen => 20; // for status, date, folderid,  What more?

        [Kooboo.Data.Attributes.KIgnore]
        public long MetaKey { get; set; }
        [Kooboo.Data.Attributes.KIgnore]
        public bool SkipValueBlock => false;
        [Kooboo.Data.Attributes.KIgnore]
        public void ParseMetaBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 20)
            {
                return;
            }
            byte[] flag = new byte[1];
            flag[0] = bytes[0];
            BitArray array = new BitArray(flag);
            this.Deleted = array.Get(0);
            this.Read = array.Get(1);
            this.Flagged = array.Get(2);
            this.Answered = array.Get(3);
            this.Recent = array.Get(4);

            this.AddressId = BitConverter.ToInt32(bytes, 4);
            this.FolderId = BitConverter.ToInt32(bytes, 8);
            this.Size = BitConverter.ToInt32(bytes, 12);

            int dateint = BitConverter.ToInt32(bytes, 16);

            this.CreationTime = Lib.Helper.DateTimeHelper.Int32ToDate(dateint);
        }

        //0-4 = flag & reserver.
        //4-8: AddressId. 
        // 8-12: FolderId 
        // 12-16: MessageDate. 
        // 16-20: size
        [Kooboo.Data.Attributes.KIgnore]
        public byte[] GetMetaBytes()
        {
            byte[] bytes = new byte[20];

            BitArray array = new BitArray(8);
            //public bool Read { get; set; } 
            //public bool Answered { get; set; } 
            //public bool Deleted { get; set; } 
            //public bool Flagged { get; set; } 
            array.Set(0, this.Deleted);
            array.Set(1, this.Read);
            array.Set(2, this.Flagged);
            array.Set(3, this.Answered);
            array.Set(4, this.Recent);
            array.CopyTo(bytes, 0);

            System.Buffer.BlockCopy(BitConverter.GetBytes(this.AddressId), 0, bytes, 4, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.FolderId), 0, bytes, 8, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(this.Size), 0, bytes, 12, 4);

            var intdate = Lib.Helper.DateTimeHelper.DateToInt32(this.CreationTime);
            System.Buffer.BlockCopy(BitConverter.GetBytes(intdate), 0, bytes, 16, 4);

            return bytes;
        }
    }

}
