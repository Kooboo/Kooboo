//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.Imap
{
    public static class ImapHelperExtensions
    {
        public static string[] GetFlags(this Message message)
        {
            var flags = new List<string>();
            if (message.Recent)
            {
                flags.Add("Recent");
            }
            if (message.Read)
            {
                flags.Add("Seen");
            }
            if (message.Answered)
            {
                flags.Add("Answered");
            }
            if (message.Flagged)
            {
                flags.Add("Flagged");
            }
            if (message.Deleted)
            {
                flags.Add("Deleted");
            }
            return flags.ToArray();
        }

        public static void AddFlags(this Message message, string[] flags)
        {
            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                message.Read = true;
            }
            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                message.Answered = true;
            }
            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                message.Flagged = true;
            }
            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                message.Deleted = true;
            }
        }

        public static void ReplaceFlags(this Message message, string[] flags)
        {
            message.Read = flags.Contains("Seen", StringComparer.OrdinalIgnoreCase);
            message.Answered = flags.Contains("Answered", StringComparer.OrdinalIgnoreCase);
            message.Flagged = flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase);
            message.Deleted = flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase);
        }

        public static void RemoveFlags(this Message message, string[] flags)
        {
            if (flags.Contains("Seen", StringComparer.OrdinalIgnoreCase))
            {
                message.Read = false;
            }
            if (flags.Contains("Answered", StringComparer.OrdinalIgnoreCase))
            {
                message.Answered = false;
            }
            if (flags.Contains("Flagged", StringComparer.OrdinalIgnoreCase))
            {
                message.Flagged = false;
            }
            if (flags.Contains("Deleted", StringComparer.OrdinalIgnoreCase))
            {
                message.Deleted = false;
            }
        }

    }
}
