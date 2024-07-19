//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Imap.Commands
{
    public class ListCommand : ICommand
    {
        public string AdditionalResponse
        {
            get; set;
        }

        public virtual string CommandName
        {
            get
            {
                return "LIST";
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

            string[] parts = TextUtils.SplitQuotedString(args, ' ', true);

            string refName = IMAP_Utils.DecodeMailbox(parts[0]);
            string folder = IMAP_Utils.DecodeMailbox(parts[1]);

            // mailbox name is "", return delimiter and root
            if (folder == String.Empty)
            {
                var root = refName.Split(new char[] { '/' })[0];
                return Task.FromResult(new List<ImapResponse>
                {
                    new ImapResponse(ResultLine.LIST(root, '/', null))
                });
            }

            // Match the full mailbox pattern
            var folderPattern = folder.Replace("*", ".*").Replace("%", "[^/]*");
            var pattern = $"^{refName}{folderPattern}$";

            var user = Data.GlobalDb.Users.Get(session.AuthenticatedUserIdentity.Name);
            var result = new List<ImapResponse>();

            var orgId = session.MailDb.OrganizationId;

            foreach (var each in GetAllFolders(user, orgId))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(each.Name, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    result.Add(new ImapResponse(Response(each.Name, '/', each.Attributes)));
                }
            }

            return Task.FromResult(result);
        }

        protected virtual List<ImapFolder> GetAllFolders(Data.Models.User user, Guid LoginOrganizationId)
        {
            return ImapHelper.GetAllFolder(user, LoginOrganizationId);
        }

        protected virtual string Response(string folderName, char delimiter, List<string> attributes)
        {
            return ResultLine.LIST(folderName, delimiter, attributes);
        }
    }
}


//6.3.8.  LIST Command

//   Arguments:  reference name
//               mailbox name with possible wildcards

//   Responses:  untagged responses: LIST

//   Result:     OK - list completed
//               NO - list failure: can't list that reference or name
//               BAD - command unknown or arguments invalid

//      The LIST command returns a subset of names from the complete set
//      of all names available to the client.Zero or more untagged LIST
//      replies are returned, containing the name attributes, hierarchy
//      delimiter, and name; see the description of the LIST reply for
//      more detail.

//      The LIST command SHOULD return its data quickly, without undue
//      delay.For example, it SHOULD NOT go to excess trouble to
//      calculate the \Marked or \Unmarked status or perform other
//      processing; if each name requires 1 second of processing, then a
//      list of 1200 names would take 20 minutes!

//      An empty("" string) reference name argument indicates that the
//      mailbox name is interpreted as by SELECT.The returned mailbox
//      names MUST match the supplied mailbox name pattern.  A non-empty
//      reference name argument is the name of a mailbox or a level of
//      mailbox hierarchy, and indicates the context in which the mailbox
//      name is interpreted.

//      An empty ("" string) mailbox name argument is a special request to
//      return the hierarchy delimiter and the root name of the name given
//      in the reference.The value returned as the root MAY be the empty
//      string if the reference is non-rooted or is an empty string.  In
//      all cases, a hierarchy delimiter (or NIL if there is no hierarchy)
//      is returned.This permits a client to get the hierarchy delimiter
//    (or find out that the mailbox names are flat) even when no
//    mailboxes by that name currently exist.

//    The reference and mailbox name arguments are interpreted into a
//      canonical form that represents an unambiguous left-to-right
//      hierarchy.The returned mailbox names will be in the interpreted
//      form.





//Crispin Standards Track[Page 40]

//RFC 3501                         IMAPv4 March 2003


//           Note: The interpretation of the reference argument is
//           implementation-defined.It depends upon whether the
//           server implementation has a concept of the "current
//           working directory" and leading "break out characters",
//           which override the current working directory.

//           For example, on a server which exports a UNIX or NT
//           filesystem, the reference argument contains the current
//           working directory, and the mailbox name argument would
//           contain the name as interpreted in the current working
//           directory.

//           If a server implementation has no concept of break out
//           characters, the canonical form is normally the reference
//           name appended with the mailbox name.Note that if the
//           server implements the namespace convention (section
//           5.1.2), "#" is a break out character and must be treated
//           as such.

//           If the reference argument is not a level of mailbox
//           hierarchy(that is, it is a \NoInferiors name), and/or
//          the reference argument does not end with the hierarchy
//           delimiter, it is implementation-dependent how this is
//           interpreted.For example, a reference of "foo/bar" and
//           mailbox name of "rag/baz" could be interpreted as
//           "foo/bar/rag/baz", "foo/barrag/baz", or "foo/rag/baz".
//           A client SHOULD NOT use such a reference argument except
//           at the explicit request of the user.  A hierarchical
//           browser MUST NOT make any assumptions about server
//           interpretation of the reference unless the reference is
//           a level of mailbox hierarchy AND ends with the hierarchy
//           delimiter.

//      Any part of the reference argument that is included in the
//      interpreted form SHOULD prefix the interpreted form.  It SHOULD
//      also be in the same form as the reference name argument.  This
//      rule permits the client to determine if the returned mailbox name
//      is in the context of the reference argument, or if something about
//      the mailbox argument overrode the reference argument.Without
//      this rule, the client would have to have knowledge of the server's
//      naming semantics including what characters are "breakouts" that
//      override a naming context.


//Crispin Standards Track[Page 41]

//RFC 3501                         IMAPv4 March 2003


//           For example, here are some examples of how references
//           and mailbox names might be interpreted on a UNIX-based
//           server:

//               Reference Mailbox Name Interpretation
//               ------------  ------------  --------------
//               ~smith/Mail/  foo.*         ~smith/Mail/foo.*
//               archive/      %             archive/%
//               #news.        comp.mail.*   #news.comp.mail.*
//               ~smith/Mail/  /usr/doc/foo  /usr/doc/foo
//               archive/      ~fred/Mail/*  ~fred/Mail/*

//           The first three examples demonstrate interpretations in
//           the context of the reference argument.  Note that
//           "~smith/Mail" SHOULD NOT be transformed into something
//           like "/u2/users/smith/Mail", or it would be impossible
//           for the client to determine that the interpretation was
//           in the context of the reference.

//      The character "*" is a wildcard, and matches zero or more
//      characters at this position.  The character "%" is similar to "*",
//      but it does not match a hierarchy delimiter.  If the "%" wildcard
//      is the last character of a mailbox name argument, matching levels
//      of hierarchy are also returned.  If these levels of hierarchy are
//      not also selectable mailboxes, they are returned with the
//      \Noselect mailbox name attribute (see the description of the LIST
//      response for more details).

//      Server implementations are permitted to "hide" otherwise
//      accessible mailboxes from the wildcard characters, by preventing
//      certain characters or names from matching a wildcard in certain
//      situations.  For example, a UNIX-based server might restrict the
//      interpretation of "*" so that an initial "/" character does not
//      match.

//      The special name INBOX is included in the output from LIST, if
//      INBOX is supported by this server for this user and if the
//      uppercase string "INBOX" matches the interpreted reference and
//      mailbox name arguments with wildcards as described above.  The
//      criteria for omitting INBOX is whether SELECT INBOX will return
//      failure; it is not relevant whether the user's real INBOX resides
//      on this or some other server.









//Crispin                     Standards Track                    [Page 42]

//RFC 3501                         IMAPv4                       March 2003


//   Example:    C: A101 LIST "" ""
//               S: * LIST (\Noselect) "/" ""
//               S: A101 OK LIST Completed
//               C: A102 LIST #news.comp.mail.misc ""
//               S: * LIST (\Noselect) "." #news.
//               S: A102 OK LIST Completed
//               C: A103 LIST /usr/staff/jones ""
//               S: * LIST (\Noselect) "/" /
//               S: A103 OK LIST Completed
//               C: A202 LIST ~/Mail/ %
//               S: * LIST (\Noselect) "/" ~/Mail/foo
//               S: * LIST () "/" ~/Mail/meetings
//               S: A202 OK LIST completed

