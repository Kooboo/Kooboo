//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Imap.Commands
{
    public class CREATE : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "CREATE";
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
            var folderName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(args));

            var specialCharPattern = @"^[a-zA-Z0-9-_.\u4e00-\u9fa5]{1,50}$";
            if (!Regex.IsMatch(folderName, specialCharPattern))
                throw new CommandException("NO", "Folder names can not contain special characters when creating.");

            if (Folder.ReservedFolder.Any(o => folderName.StartsWith(o.Value, StringComparison.OrdinalIgnoreCase)))
                throw new CommandException("NO", "Can not create folder under reservered folders");

            if (session.MailDb.Folder.Get(folderName) != null)
                throw new CommandException("NO", "Folder already exist");

            session.MailDb.Folder.Add(folderName);

            return this.NullResult();
        }
    }
}



//6.3.3.  CREATE Command

//   Arguments:  mailbox name

//   Responses:  no specific responses for this command

//   Result:     OK - create completed
//               NO - create failure: can't create mailbox with that name
//               BAD - command unknown or arguments invalid

//      The CREATE command creates a mailbox with the given name.An OK
//      response is returned only if a new mailbox with that name has been
//      created.It is an error to attempt to create INBOX or a mailbox
//      with a name that refers to an extant mailbox.Any error in
//      creation will return a tagged NO response.



//Crispin                     Standards Track                    [Page 34]

//RFC 3501                         IMAPv4 March 2003


//      If the mailbox name is suffixed with the server's hierarchy
//      separator character (as returned from the server by a LIST
//      command), this is a declaration that the client intends to create
//      mailbox names under this name in the hierarchy.  Server
//      implementations that do not require this declaration MUST ignore
//      the declaration.In any case, the name created is without the
//      trailing hierarchy delimiter.

//      If the server's hierarchy separator character appears elsewhere in
//      the name, the server SHOULD create any superior hierarchical names
//      that are needed for the CREATE command to be successfully
//      completed.In other words, an attempt to create "foo/bar/zap" on
//      a server in which "/" is the hierarchy separator character SHOULD
//      create foo/ and foo/bar/ if they do not already exist.

//      If a new mailbox is created with the same name as a mailbox which
//      was deleted, its unique identifiers MUST be greater than any
//      unique identifiers used in the previous incarnation of the mailbox
//      UNLESS the new incarnation has a different unique identifier
//      validity value.See the description of the UID command for more
//      detail.

//   Example:    C: A003 CREATE owatagusiam/
//               S: A003 OK CREATE completed
//               C: A004 CREATE owatagusiam/blurdybloop
//               S: A004 OK CREATE completed

//        Note: The interpretation of this example depends on whether
//        "/" was returned as the hierarchy separator from LIST.If
//        "/" is the hierarchy separator, a new level of hierarchy
//        named "owatagusiam" with a member called "blurdybloop" is
//        created.Otherwise, two mailboxes at the same hierarchy
//        level are created.
