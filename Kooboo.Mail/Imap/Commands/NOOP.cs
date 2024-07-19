//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Kooboo.Mail.Imap.Commands
{
    public class NOOP : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public virtual string CommandName
        {
            get
            {
                return "NOOP";
            }
        }

        public virtual bool RequireAuth
        {
            get
            {
                return false;
            }
        }

        public virtual bool RequireFolder
        {
            get
            {
                return false;
            }
        }

        public bool RequireTwoPartCommand
        {
            get
            {
                return false;
            }
        }

        public Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            List<ImapResponse> result = new List<ImapResponse>();

            if (session.SelectFolder == null)
                return this.NullResult();

            var deletedSeqNo = session.SelectFolder.EXPUNGE();

            foreach (var item in deletedSeqNo)
            {
                result.Add(new ImapResponse(ResultLine.EXPUNGE(item)));
            }

            //session.SelectFolder.Stat = session.MailDb.Msgstore.GetStat(session.SelectFolder.Folder);
            session.SelectFolder.Stat = session.MailDb.Message2.GetStat(session.SelectFolder.FolderId, session.SelectFolder.AddressId);

            var stat = session.SelectFolder.Stat;

            result.Add(new ImapResponse(ResultLine.EXISTS(stat.Exists)));
            result.Add(new ImapResponse(ResultLine.RECENT(stat.Recent)));

            result.Add(new ImapResponse(ResultLine.UIDNEXT(stat.NextUid)));

            result.Add(new ImapResponse(ResultLine.UIDVALIDAITY(stat.UIDVALIDITY)));

            return Task.FromResult(result);
        }

    }
}


/* RFC 3501 6.1.2. NOOP Command.
         Arguments:  none

         Responses:  no specific responses for this command (but see below)

         Result:     OK - noop completed
                     BAD - command unknown or arguments invalid

         The NOOP command always succeeds.  It does nothing.

         Since any command can return a status update as untagged data, the
         NOOP command can be used as a periodic poll for new messages or
         message status updates during a period of inactivity (this is the
         preferred method to do this).  The NOOP command can also be used
         to reset any inactivity autologout timer on the server.

         Example:    C: a002 NOOP
                     S: a002 OK NOOP completed
                     . . .
                     C: a047 NOOP
                     S: * 22 EXPUNGE
                     S: * 23 EXISTS
                     S: * 3 RECENT
                     S: * 14 FETCH (FLAGS (\Seen \Deleted))
                     S: a047 OK NOOP completed
     */

//if (m_pSelectedFolder != null)
//{
//    UpdateSelectedFolderAndSendChanges();
//}

//// m_pResponseSender.SendResponseAsync(new IMAP_r_(cmdTag, "OK", "NOOP Completed"));

//m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "NOOP Completed"));