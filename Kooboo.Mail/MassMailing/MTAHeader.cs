namespace Kooboo.Mail.MassMailing
{
    public class MTAHeader
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; } // to be signed..  
        public string Date { get; set; }   //  to be replaced and signed. 

        public XCommand Command { get; set; }
    }
}
