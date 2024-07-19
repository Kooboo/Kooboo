//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public class EXPUNGE : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "EXPUNGE";
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
            List<ImapResponse> result = new List<ImapResponse>();

            var deletedSeqNo = session.SelectFolder.EXPUNGE();

            foreach (var item in deletedSeqNo)
            {
                result.Add(new ImapResponse(ResultLine.EXPUNGE(item)));
            }
            return Task.FromResult(result);
        }
    }
}



//6.4.3.  EXPUNGE Command

//   Arguments:  none

//   Responses:  untagged responses: EXPUNGE

//   Result:     OK - expunge completed
//               NO - expunge failure: can't expunge (e.g., permission
//                    denied)
//               BAD - command unknown or arguments invalid

//      The EXPUNGE command permanently removes all messages that have the
//      \Deleted flag set from the currently selected mailbox.Before
//      returning an OK to the client, an untagged EXPUNGE response is
//      sent for each message that is removed.

//   Example:    C: A202 EXPUNGE
//               S: * 3 EXPUNGE
//               S: * 3 EXPUNGE
//               S: * 5 EXPUNGE
//               S: * 8 EXPUNGE
//               S: A202 OK EXPUNGE completed

//        Note: In this example, messages 3, 4, 7, and 11 had the
//        \Deleted flag set.See the description of the EXPUNGE
//        response for further explanation.