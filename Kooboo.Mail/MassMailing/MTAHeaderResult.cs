namespace Kooboo.Mail.MassMailing
{
    public class MTAHeaderResult
    {
        public MTAHeader MtaHeader { get; set; }

        public DkimHeaderLine DkimLine { get; set; }

        public string NewHeader { get; set; }

        public string Body { get; set; }  // include two r\n \r\n.  
    }



}
