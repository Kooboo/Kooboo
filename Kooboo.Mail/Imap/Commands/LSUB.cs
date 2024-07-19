//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Imap.Commands
{
    public class LSUB : ListCommand
    {
        public override string CommandName
        {
            get
            {
                return "LSUB";
            }
        }

        protected override List<ImapFolder> GetAllFolders(Data.Models.User user, Guid LoginOrganizationId)
        {

            return ImapHelper.GetSubscribedFolder(user, LoginOrganizationId);
        }

        protected override string Response(string folderName, char delimiter, List<string> attributes)
        {
            return ResultLine.LSUB(folderName, delimiter, attributes);
        }
    }
}

//6.3.9.  LSUB Command

//   Arguments:  reference name
//               mailbox name with possible wildcards

//   Responses:  untagged responses: LSUB

//   Result:     OK - lsub completed
//               NO - lsub failure: can't list that reference or name
//               BAD - command unknown or arguments invalid

//      The LSUB command returns a subset of names from the set of names
//      that the user has declared as being "active" or "subscribed".
//      Zero or more untagged LSUB replies are returned.The arguments to
//      LSUB are in the same form as those for LIST.

//      The returned untagged LSUB response MAY contain different mailbox
//      flags from a LIST untagged response.If this should happen, the
//      flags in the untagged LIST are considered more authoritative.

//      A special situation occurs when using LSUB with the % wildcard.
//      Consider what happens if "foo/bar" (with a hierarchy delimiter of
//      "/") is subscribed but "foo" is not.A "%" wildcard to LSUB must
//      return foo, not foo/bar, in the LSUB response, and it MUST be
//      flagged with the \Noselect attribute.

//      The server MUST NOT unilaterally remove an existing mailbox name
//      from the subscription list even if a mailbox by that name no
//      longer exists.







//Crispin Standards Track[Page 43]

//RFC 3501                         IMAPv4 March 2003


//   Example:    C: A002 LSUB "#news." "comp.mail.*"
//               S: * LSUB () "." #news.comp.mail.mime
//               S: * LSUB() "." #news.comp.mail.misc
//               S: A002 OK LSUB completed
//               C: A003 LSUB "#news." "comp.%"
//               S: * LSUB(\NoSelect) "." #news.comp.mail
//               S: A003 OK LSUB completed
