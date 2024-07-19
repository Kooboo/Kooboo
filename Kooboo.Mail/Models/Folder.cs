//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Mail
{
    /// <summary>
    /// Folder belong to user. one user can create mutiple folders. 
    /// Folder can be used to store message from one or more email address. 
    /// 
    /// </summary>
    public class Folder : IMailObject
    {
        public const string Inbox = "Inbox";
        public const string Drafts = "Drafts";
        public const string Sent = "Sent";
        public const string Spam = "Spam";
        public const string Trash = "Trash";


        private static Dictionary<int, string> _ReservedFolder;
        public static Dictionary<int, string> ReservedFolder
        {
            get
            {
                if (_ReservedFolder == null)
                {
                    _ReservedFolder = new Dictionary<int, string>
                    {
                        { ToId(Inbox), Inbox },
                        { ToId(Drafts), Drafts },
                        { ToId(Sent), Sent },
                        { ToId(Spam), Spam },
                        { ToId(Trash), Trash }
                    };
                }
                return _ReservedFolder;
            }
        }

        public Folder()
        {

        }

        public Folder(string name)
        {
            this.Name = name;
        }

        private int _id;
        public int Id
        {
            get
            {
                if (_id == default)
                {
                    _id = ToId(Name);
                }
                return _id;
            }
            set { _id = value; }
        }

        // name with heriachy. 
        public string Name { get; set; }

        /// <summary>
        /// IMAP中的订阅概念会用到
        /// </summary>
        public bool Subscribed { get; set; } = true;

        public static int ToId(string name)
        {
            return Helper.IDHelper.ToInt(name);
        }

        public override int GetHashCode()
        {
            string unique = this.Name + this.Subscribed.ToString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
