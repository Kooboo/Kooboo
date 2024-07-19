namespace Kooboo.Web.Api.Implementation.Mails.ViewModel
{
    public class MailAdvancedSearch
    {
        public string Keyword { get; set; }
        public string Position { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateType DateType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SearchFolder { get; set; }
        public int ReadOrUnRead { get; set; }
    }

    public enum PositionType
    {
        Subject,
        Body,
        Attachments,
        AllContents,
    }

    public enum DateType : int
    {
        NoLimit = 0,
        OneDay = 1,
        ThreeDay = 2,
        OneWeek = 3,
        TwoWeek = 4,
        OneMonth = 5,
        SelfDefinition = 7
    }

    public enum FolderType
    {
        Inbox,
        Drafts,
        Sent,
        Spam,
        Trash,
        SelfDefinition,
        AllEmails
    }
}
