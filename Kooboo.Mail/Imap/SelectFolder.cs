//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.Imap
{
    public class SelectFolder
    {
        private MailDb mailDb { get; set; }

        public Models.MessageStat Stat { get; set; }

        public int FolderId { get; set; }

        public int AddressId { get; set; }

        public SelectFolder(string folder, MailDb maildb)
        {
            if (folder == null)
            {
                throw new CommandException("BAD", "Folder not provided");
            }
            this.mailDb = maildb;
            this.Folder = folder;

            var prasedfolder = Utility.FolderUtility.ParseFolder(folder);

            var dbfolder = this.mailDb.Folder.Get(prasedfolder.FolderId);

            if (dbfolder == null)
            {
                throw new CommandException("NO", "No such a folder");
            }

            this.FolderId = prasedfolder.FolderId;
            this.AddressId = prasedfolder.AddressId;

            // this.Stat =this.mailDb.Msgstore.GetStat(this.FolderId, this.AddressId);
            this.Stat = this.mailDb.Message2.GetStat(this.FolderId, this.AddressId);
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
            List<Message> all = null;

            if (this.AddressId != 0)
            {
                all = this.mailDb.Message2.Query.Where(o => o.FolderId == this.FolderId && o.AddressId == this.AddressId && o.Deleted).SelectAll().ToList();
            }
            else

            {
                all = this.mailDb.Message2.Query.Where(o => o.FolderId == this.FolderId && o.Deleted).SelectAll().ToList();
            }

            List<int> result = new List<int>();

            foreach (var item in all)
            {
                var seqno = this.mailDb.Message2.GetSeqNo(this, item.MsgId);
                result.Add(seqno);
            }

            foreach (var item in all)
            {
                this.mailDb.Message2.Delete(item.MsgId);
            }
            this.Stat = this.mailDb.Message2.GetStat(this.FolderId, this.AddressId);

            return result;
        }

        internal void Reset()
        {
            this.Stat = mailDb.Message2.GetStat(this.FolderId, this.AddressId);
        }

        #region Properties implementation

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder
        {
            get; set;
        }

        #endregion

    }


}
