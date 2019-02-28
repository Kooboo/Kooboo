//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Kooboo.Mail;

namespace Kooboo.Mail
{
    public class Message : IMailObject
    {
        public Message()
        { 
  
        } 

        public int Id { get; set; }

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

        public string  Bcc { get; set; }

        public string Subject { get; set; }

        public long BodyPosition { get; set;  }
      
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
                _creationTimetick = default(long); 
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
        public List<Models.Attachment> Attachments {
            get
            {
                if (_attachments ==  null)
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
                if(_attachments !=null && _attachments.Count()>0)
                {
                    return true; 
                }
                return false; 
            }
        }
        
    }

   
}
