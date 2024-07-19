//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Imap
{
    public static class Setting
    {
        private static string[] _flags;

        public static string[] SupportFlags
        {
            get
            {
                if (_flags == null)
                {
                    //\Answered \Flagged \Deleted \Seen \Draft
                    _flags = new string[] { "Seen", "Answered", "Flagged", "Deleted" };

                }
                return _flags;
            }
        }

        //PERMANENTFLAGS
        private static string[] _permanentflags;
        public static string[] PermanentFlags
        {
            get
            {
                if (_permanentflags == null)
                {
                    //\Answered \Flagged \Deleted \Seen \Draft
                    _permanentflags = new string[] { "Seen", "Answered", "Flagged", "Deleted" };

                }
                return _permanentflags;
            }
        }

    }
}
