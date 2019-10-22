//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Mail.Imap
{
    public static class Setting
    {
        private static string[] _flags;

        public static string[] SupportFlags
        {
            get { return _flags ?? (_flags = new string[] {"Seen", "Answered", "Flagged", "Deleted"}); }
        }

        //PERMANENTFLAGS
        private static string[] _permanentflags;

        public static string[] PermanentFlags
        {
            get
            {
                return _permanentflags ?? (_permanentflags = new string[] {"Seen", "Answered", "Flagged", "Deleted"});
            }
        }
    }
}