//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;


namespace Kooboo.Mail.Imap.Commands
{
    public class DELETE : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public string CommandName
        {
            get
            {
                return "DELETE";
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

            var prasedFolder = Utility.FolderUtility.ParseFolder(folderName);

            if (prasedFolder.AddressId != 0)
            {
                var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(session.MailDb.OrganizationId);
                orgdb.Email.Delete(prasedFolder.AddressId);
                return this.NullResult();
            }

            if (Folder.ReservedFolder.ContainsKey(prasedFolder.FolderId))
            {
                throw new CommandException("NO", "Can not delete reservered folders");
            }

            var folder = session.MailDb.Folder.Get(folderName);
            if (folder == null)
                throw new CommandException("NO", "Folder not found");

            try
            {
                session.MailDb.Folder.Delete(folder);
            }
            catch (System.Exception ex)
            {
                throw new CommandException("NO", ex.Message);
            }

            if (session.SelectFolder != null && session.SelectFolder.FolderId == folder.Id)
            {
                session.SelectFolder = null;
            }

            return this.NullResult();
        }
    }
}



//6.3.4.  DELETE Command

//   Arguments:  mailbox name

//   Responses:  no specific responses for this command

//   Result:     OK - delete completed
//               NO - delete failure: can't delete mailbox with that name
//               BAD - command unknown or arguments invalid







//Crispin                     Standards Track                    [Page 35]

//RFC 3501                         IMAPv4 March 2003


//      The DELETE command permanently removes the mailbox with the given
//      name.A tagged OK response is returned only if the mailbox has
//      been deleted.It is an error to attempt to delete INBOX or a
//      mailbox name that does not exist.

//      The DELETE command MUST NOT remove inferior hierarchical names.
//      For example, if a mailbox "foo" has an inferior "foo.bar"
//      (assuming "." is the hierarchy delimiter character), removing
//      "foo" MUST NOT remove "foo.bar".  It is an error to attempt to
//      delete a name that has inferior hierarchical names and also has
//      the \Noselect mailbox name attribute (see the description of the
//      LIST response for more details).

//      It is permitted to delete a name that has inferior hierarchical
//      names and does not have the \Noselect mailbox name attribute.  In
//      this case, all messages in that mailbox are removed, and the name
//      will acquire the \Noselect mailbox name attribute.

//      The value of the highest-used unique identifier of the deleted
//      mailbox MUST be preserved so that a new mailbox created with the
//      same name will not reuse the identifiers of the former
//      incarnation, UNLESS the new incarnation has a different unique
//      identifier validity value.See the description of the UID command
//      for more detail.

//   Examples:   C: A682 LIST "" *
//               S: * LIST () "/" blurdybloop
//               S: * LIST(\Noselect) "/" foo
//              S: * LIST() "/" foo/bar
//             S: A682 OK LIST completed
//               C: A683 DELETE blurdybloop
//               S: A683 OK DELETE completed
//               C: A684 DELETE foo
//               S: A684 NO Name "foo" has inferior hierarchical names
//               C: A685 DELETE foo/bar
//               S: A685 OK DELETE Completed
//               C: A686 LIST "" *
//               S: * LIST(\Noselect) "/" foo
//              S: A686 OK LIST completed
//               C: A687 DELETE foo
//               S: A687 OK DELETE Completed










//Crispin Standards Track[Page 36]

//RFC 3501                         IMAPv4 March 2003


//               C: A82 LIST "" *
//               S: * LIST() "." blurdybloop
//              S: * LIST() "." foo
//             S: * LIST() "." foo.bar
//            S: A82 OK LIST completed
//               C: A83 DELETE blurdybloop
//               S: A83 OK DELETE completed
//               C: A84 DELETE foo
//               S: A84 OK DELETE Completed
//               C: A85 LIST "" *
//               S: * LIST() "." foo.bar
//              S: A85 OK LIST completed
//               C: A86 LIST "" %
//               S: * LIST(\Noselect) "." foo
//              S: A86 OK LIST completed
