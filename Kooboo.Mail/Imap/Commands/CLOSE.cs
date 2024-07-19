//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public class CLOSE : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "CLOSE";
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
                return true;
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
            session.SelectFolder.EXPUNGE();

            return this.NullResult();
        }
    }
}

//6.4.2.  CLOSE Command

//   Arguments:  none

//   Responses:  no specific responses for this command

//   Result:     OK - close completed, now in authenticated state
//               BAD - command unknown or arguments invalid

//      The CLOSE command permanently removes all messages that have the
//      \Deleted flag set from the currently selected mailbox, and returns
//      to the authenticated state from the selected state.  No untagged
//      EXPUNGE responses are sent.

//      No messages are removed, and no error is given, if the mailbox is
//      selected by an EXAMINE command or is otherwise selected read-only.

//      Even if a mailbox is selected, a SELECT, EXAMINE, or LOGOUT
//      command MAY be issued without previously issuing a CLOSE command.
//      The SELECT, EXAMINE, and LOGOUT commands implicitly close the
//      currently selected mailbox without doing an expunge.However,
//      when many messages are deleted, a CLOSE-LOGOUT or CLOSE-SELECT
//      sequence is considerably faster than an EXPUNGE-LOGOUT or
//      EXPUNGE-SELECT because no untagged EXPUNGE responses (which the
//      client would probably ignore) are sent.

//   Example:    C: A341 CLOSE
//               S: A341 OK CLOSE completed