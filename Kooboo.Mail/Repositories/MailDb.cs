//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB;
using Kooboo.Data;
using Kooboo.Mail.Repositories;

namespace Kooboo.Mail
{
    public  class MailDb : IDisposable
    {
        private object _lock = new object();
   
        public MailDb(Guid UserId, Guid OrganizationId)
        {
            this.UserId = UserId;
            string folder = AppSettings.GetMailDbName(OrganizationId);
            string dbname = System.IO.Path.Combine(folder, UserId.ToString());
            Db = DB.GetDatabase(dbname);
        }

        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public Database Db { get; set; }

        private FolderRepository _folders;
        public FolderRepository Folders
        {
            get
            {
                if (_folders == null)
                {
                    lock(_lock)
                    {
                        if (_folders == null)
                        {
                            _folders = new FolderRepository(this); 
                        }
                    }
                }
                return _folders; 
            }
        }

        private MessageRepository _messages;
        public MessageRepository Messages
        {
            get
            {
                if (_messages == null)
                {
                    lock(_lock)
                    {
                        if (_messages == null)
                        {
                            _messages = new MessageRepository(this); 
                        }
                    }
                } 
                return _messages;  
            }
        }

        private TargetAddressRepository _targetAddresses;
        public TargetAddressRepository TargetAddresses
        {
            get
            {
                if (_targetAddresses == null)
                {
                    lock(_lock)
                    {
                        if (_targetAddresses == null)
                        {
                            _targetAddresses = new TargetAddressRepository(this); 
                        }
                    }
                }
                return _targetAddresses;  
            }
        }

        private Sequence<string> _msgbody; 
        public Kooboo.IndexedDB.Sequence<string> MsgBody
        {
            get
            {
                if (_msgbody == null)
                {
                    lock(_lock)
                    {
                        _msgbody = this.Db.GetSequence<string>("MessageSource"); 
                    }
                }
                return _msgbody;
            }
        }
          
        public void Dispose()
        {
            this.Db.Close(); 
        }
    }
}
