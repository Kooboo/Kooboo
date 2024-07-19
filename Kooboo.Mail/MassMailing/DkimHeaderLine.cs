namespace Kooboo.Mail.MassMailing
{
    //For easy implementation, we limit sign header tp from, to, subject, date
    public record DkimHeaderLine
    {
        public string FromLine { get; set; }

        public string ToLine { get; set; }

        public string SubjectLine { get; set; }

        public string Dateline { get; set; }

    }



}
