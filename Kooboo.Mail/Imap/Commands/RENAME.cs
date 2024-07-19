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
    public class RENAME : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "RENAME";
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
                return true;
            }
        }

        public Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            var parts = TextUtils.SplitQuotedString(args, ' ', false, 2);

            var fromName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(parts[0]));
            var toName = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(parts[1]));

            var specialCharPattern = @"^[a-zA-Z0-9-_.\/\u4e00-\u9fa5]{1,50}$";
            if (!Regex.IsMatch(toName, specialCharPattern))
                throw new CommandException("NO", "Folder names can not contain special characters when renaming.");

            if (Folder.ReservedFolder.Any(o => fromName.StartsWith(o.Value, StringComparison.OrdinalIgnoreCase)))
                throw new CommandException("NO", "Can not rename reservered folders");

            var folder = session.MailDb.Folder.Get(fromName);
            if (folder == null)
                throw new CommandException("NO", "Folder  not found");

            if (Folder.ReservedFolder.Any(o => toName.StartsWith(o.Value, StringComparison.OrdinalIgnoreCase)))
                throw new CommandException("NO", "Can not rename folder under reservered folders");

            if (session.MailDb.Folder.Get(toName) != null)
                throw new CommandException("NO", "Folder with new name already exist");

            session.MailDb.Folder.Rename(folder, toName);

            return this.NullResult();
        }
    }
}



//6.2.3.  LOGIN Command

//   Arguments:  user name
//               password

//   Responses:  no specific responses for this command

//   Result:     OK - login completed, now in authenticated state
//               NO - login failure: user name or password rejected
//               BAD - command unknown or arguments invalid

//      The LOGIN command identifies the client to the server and carries
//      the plaintext password authenticating this user.






//Crispin Standards Track[Page 30]

//RFC 3501                         IMAPv4 March 2003


//      A server MAY include a CAPABILITY response code in the tagged OK
//      response to a successful LOGIN command in order to send
//      capabilities automatically.It is unnecessary for a client to
//      send a separate CAPABILITY command if it recognizes these
//      automatic capabilities.

//   Example:    C: a001 LOGIN SMITH SESAME
//               S: a001 OK LOGIN completed

//        Note: Use of the LOGIN command over an insecure network
//        (such as the Internet) is a security risk, because anyone
//        monitoring network traffic can obtain plaintext passwords.
//        The LOGIN command SHOULD NOT be used except as a last
//        resort, and it is recommended that client implementations
//        have a means to disable any automatic use of the LOGIN
//        command.

//        Unless either the STARTTLS command has been negotiated or
//        some other mechanism that protects the session from
//        password snooping has been provided, a server
//        implementation MUST implement a configuration in which it
//        advertises the LOGINDISABLED capability and does NOT permit
//        the LOGIN command.  Server sites SHOULD NOT use any
//        configuration which permits the LOGIN command without such
//        a protection mechanism against password snooping.  A client
//        implementation MUST NOT send a LOGIN command if the
//        LOGINDISABLED capability is advertised.