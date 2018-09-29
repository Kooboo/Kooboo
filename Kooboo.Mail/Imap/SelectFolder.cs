//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap
{
   public class SelectFolder
    {   
        private MailDb mailDb { get; set; }

        public  Models.MessageStat Stat { get; set; }

        public int FolderId { get; set; }

        private int AddressId { get; set; }

        public SelectFolder(string folder, MailDb maildb)
        {
            if (folder == null)
            {
                throw new CommandException("BAD", "Folder not provided"); 
            } 
            this.mailDb =  maildb;
            this.Folder = folder;

            var prasedfolder = Utility.FolderUtility.ParseFolder(folder);
             
            var dbfolder = this.mailDb.Folders.Get(prasedfolder.FolderId);

            if (dbfolder == null)
            {
                throw new CommandException("NO", "No such a folder");
            }

            this.FolderId = prasedfolder.FolderId;
            this.AddressId = prasedfolder.AddressId; 
             
            this.Stat =this.mailDb.Messages.GetStat(this.FolderId, this.AddressId);
        }

        public List<int> EXPUNGE()
        {
            /* RFC 3501 6.4.3. EXPUNGE Command.
         Arguments:  none

         Responses:  untagged responses: EXPUNGE

         Result:     OK - expunge completed
                     NO - expunge failure: can't expunge (e.g., permission
                          denied)
                     BAD - command unknown or arguments invalid

         The EXPUNGE command permanently removes all messages that have the
         \Deleted flag set from the currently selected mailbox.  Before
         returning an OK to the client, an untagged EXPUNGE response is
         sent for each message that is removed.

         Example:    C: A202 EXPUNGE
                     S: * 3 EXPUNGE
                     S: * 3 EXPUNGE
                     S: * 5 EXPUNGE
                     S: * 8 EXPUNGE
                     S: A202 OK EXPUNGE completed

             Note: In this example, messages 3, 4, 7, and 11 had the
             \Deleted flag set.  See the description of the EXPUNGE
             response for further explanation.
     */   
            var query = this.mailDb.Messages.Query().UseColumnData().Where(o => o.FolderId == this.FolderId).Where(o => o.Deleted == true); 
             
            if (this.AddressId != default(int))
            {
                query.Where(o => o.AddressId == this.AddressId); 
            } 
            var all = query.SelectAll();

            List<int> result = new List<int>();

            foreach (var item in all)
            {
                var seqno = this.mailDb.Messages.GetSeqNo(this.Folder, this.Stat.LastestMsgId, this.Stat.Exists, item.Id);
                result.Add(seqno); 
            }
             
            foreach (var item in all)
            {
                this.mailDb.Messages.Delete(item.Id); 
            } 
            this.Stat = this.mailDb.Messages.GetStat(this.FolderId, this.AddressId); 
            return result; 
        }

        internal void Reset()
        {
            this.Stat = mailDb.Messages.GetStat(this.Folder);
        }

        #region Properties implementation

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder
        {
            get;set;
        }

        #endregion

    }


}
