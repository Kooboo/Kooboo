//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Utility;


namespace Kooboo.Mail.Imap
{
    public class ResultLine
    {
        public static string EXPUNGE(int SeqNo)
        {
            // S: * 22 EXPUNGE
            return "* " + SeqNo.ToString() + " EXPUNGE";
        }

        public static string EXISTS(int MsgCount)
        {
            // Example:  S: * 23 EXISTS 
            return "* " + MsgCount.ToString() + " EXISTS";
        }

        public static string RECENT(int MsgCount)
        {
            // Example:    S: * 5 RECENT 
            return "* " + MsgCount.ToString() + " RECENT";
        }

        public static string UNSEEN(int firstUnseen)
        {
            return "* OK [UNSEEN " + firstUnseen.ToString() + "]";
        }

        public static string UIDNEXT(int nextuid)
        {
            return "* OK [UIDNEXT " + nextuid.ToString() + "]";
        }

        public static string UIDVALIDAITY(int UidValidityFolder)
        {
            return "* OK [UIDVALIDITY " + UidValidityFolder.ToString() + "]";
        }

        public static string FLAGS(List<string> flags)
        {
            // Example:   S: * FLAGS (\Answered \Flagged \Deleted \Seen \Draft) 
            string result = "* FLAGS (";
            if (flags != null && flags.Count() > 0)
            {
                for (int i = 0; i < flags.Count; i++)
                {
                    var value = flags[i];
                    if (!value.StartsWith("\\"))
                    {
                        flags[i] = "\\" + value;
                    }
                }
                result += string.Join(" ", flags);
            }
            result += ")";
            return result;
        }

        public static string PERMANENTFLAGS(List<string> flags)
        {
            // Example:   S: * FLAGS (\Answered \Flagged \Deleted \Seen \Draft) 
            string result = "* PERMANENTFLAGS (";
            if (flags != null && flags.Count() > 0)
            {
                for (int i = 0; i < flags.Count; i++)
                {
                    var value = flags[i];
                    if (!value.StartsWith("\\"))
                    {
                        flags[i] = "\\" + value;
                    }
                }
                result += string.Join(" ", flags);
            }
            result += ")";
            return result;
        }

        public static string LIST(string FolderName, char delimiter, List<string> attributes)
        {
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

            if (string.IsNullOrEmpty(FolderName))
            {
                return $"* LIST (\\Noselect) \"{delimiter}\" \"\"";
            }
            else
            {
                string result = "* LIST (";
                if (attributes != null && attributes.Count() > 0)
                {
                    result += string.Join(" ", attributes);
                }
                result += ") \"" + delimiter + "\" " + IMAP_Utils.EncodeMailbox(FolderName, IMAP_Mailbox_Encoding.ImapUtf7);
                return result;
            }
        }


        public static string LSUB(string FolderName, char delimiter, List<string> attributes)
        {
            // Example:    S: * LSUB (\Noselect) "/" ~/Mail/foo
            string result = "* LSUB (";

            if (attributes != null && attributes.Count() > 0)
            {
                result += string.Join(" ", attributes);
            }
            result += ") \"" + delimiter + "\" " + IMAP_Utils.EncodeMailbox(FolderName, IMAP_Mailbox_Encoding.ImapUtf7);
            return result;
        }


        public static string SEARCH(List<int> searchResults)
        {
            //7.2.5.SEARCH Response 
            //Contents:   zero or more numbers 
            //     The SEARCH response occurs as a result of a SEARCH or UID SEARCH
            //     command.The number(s)refer to those messages that match the
            //     search criteria.For SEARCH, these are message sequence numbers;
            //           for UID SEARCH, these are unique identifiers.Each number is
            //           delimited by a space. 
            //     Example:    S: *SEARCH 2 3 6

            string result = "* SEARCH ";
            if (searchResults != null && searchResults.Count() > 0)
            {
                result += string.Join(" ", searchResults.ToArray());
            }
            return result;
        }

        public static string CAPABILITY(List<string> abilities)
        {
            // Example:    S: * CAPABILITY IMAP4rev1 STARTTLS AUTH=GSSAPI XPIG-LATIN 
            string result = "* CAPABILITY";
            foreach (string capability in abilities)
            {
                result += " " + capability;
            }
            return result;
        }
    }
}
