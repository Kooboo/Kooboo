//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.Mail.Imap.Commands
{
    public class CHECK : NOOP
    {
        public override string CommandName
        {
            get
            {
                return "CHECK";
            }
        }

        public override bool RequireAuth
        {
            get
            {
                return true;
            }

        }

        public override bool RequireFolder
        {
            get
            {
                return true;
            }
        }
    }
}



//6.4.1.  CHECK Command

//   Arguments:  none

//   Responses:  no specific responses for this command

//   Result:     OK - check completed
//               BAD - command unknown or arguments invalid

//      The CHECK command requests a checkpoint of the currently selected
//      mailbox.  A checkpoint refers to any implementation-dependent
//      housekeeping associated with the mailbox (e.g., resolving the
//      server's in-memory state of the mailbox with the state on its



//Crispin Standards Track[Page 47]

//RFC 3501                         IMAPv4 March 2003


//      disk) that is not normally executed as part of each command.  A
//      checkpoint MAY take a non-instantaneous amount of real time to
//      complete.  If a server implementation has no such housekeeping
//      considerations, CHECK is equivalent to NOOP.

//      There is no guarantee that an EXISTS untagged response will happen
//      as a result of CHECK.  NOOP, not CHECK, SHOULD be used for new
//      message polling.

//   Example:    C: FXXZ CHECK
//               S: FXXZ OK CHECK Completed