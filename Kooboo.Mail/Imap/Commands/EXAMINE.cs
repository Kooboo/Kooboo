using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Imap.Commands
{

    public class EXAMINE : ICommand
    {
        public string AdditionalResponse
        {
            get
            {
                return "[READ-WRITE]";
            }
            set { }
        }

        public string CommandName
        {
            get
            {
                return "EXAMINE";
            }
        }

        public bool RequireAuth
        {
            get
            {
                return true;
            }
        }

        public bool RequireFolder
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
            string[] parts = TextUtils.SplitQuotedString(args, ' ');
            if (parts.Length >= 2)
            {
                // At moment we don't support UTF-8 mailboxes.
                if (Lib.Helper.StringHelper.IsSameValue(parts[1], "(UTF8)"))
                {
                    throw new CommandException("NO", "UTF8 name not supported");
                }
                else
                {
                    throw new CommandException("BAD", "Argument error");
                }
            }

            List<ImapResponse> Result = new List<ImapResponse>();

            string foldername = TextUtils.UnQuoteString(IMAP_Utils.DecodeMailbox(args));

            var parsedfolder = Kooboo.Mail.Utility.FolderUtility.ParseFolder(foldername);

            session.SelectFolder = new SelectFolder(foldername, session.MailDb);

            var stat = session.SelectFolder.Stat;

            //session.MailDb.Msgstore.UpdateRecentByMaxId(stat.LastestMsgId);

            Result.Add(new ImapResponse(ResultLine.EXISTS(stat.Exists)));
            Result.Add(new ImapResponse(ResultLine.RECENT(stat.Recent)));

            if (stat.FirstUnSeen > -1)
            {
                Result.Add(new ImapResponse(ResultLine.UNSEEN(stat.FirstUnSeen)));
            }

            Result.Add(new ImapResponse(ResultLine.UIDNEXT(stat.NextUid)));

            Result.Add(new ImapResponse(ResultLine.UIDVALIDAITY(stat.UIDVALIDITY)));

            Result.Add(new ImapResponse(ResultLine.FLAGS(Setting.SupportFlags.ToList())));

            return Task.FromResult(Result);
        }

    }


}

/*
///6.3.2.  EXAMINE Command

Arguments: mailbox name

   Responses: REQUIRED untagged responses: FLAGS, EXISTS, RECENT
               REQUIRED OK untagged responses:  UNSEEN,  PERMANENTFLAGS,
               UIDNEXT, UIDVALIDITY

   Result:     OK - examine completed, now in selected state
               NO - examine failure, now in authenticated state: no
                    such mailbox, can't access mailbox
               BAD - command unknown or arguments invalid

      The EXAMINE command is identical to SELECT and returns the same
      output; however, the selected mailbox is identified as read-only.
      No changes to the permanent state of the mailbox, including
      per-user state, are permitted; in particular, EXAMINE MUST NOT
      cause messages to lose the \Recent flag.

      The text of the tagged OK response to the EXAMINE command MUST
      begin with the "[READ-ONLY]" response code.

   Example:    C: A932 EXAMINE blurdybloop
               S: *17 EXISTS
               S: *2 RECENT
               S: *OK[UNSEEN 8] Message 8 is first unseen
               S: *OK[UIDVALIDITY 3857529045] UIDs valid
               S: *OK[UIDNEXT 4392] Predicted next UID
               S: *FLAGS(\Answered \Flagged \Deleted \Seen \Draft)
               S: *OK[PERMANENTFLAGS()] No permanent flags permitted
               S: A932 OK[READ - ONLY] EXAMINE completed

    */