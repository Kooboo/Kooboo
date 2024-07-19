namespace Kooboo.Mail.Models
{
    public class UnreadCounter
    {
        public int AddressId { get; set; }

        public int Exists { get; set; }

        public int Read { get; set; }

        public int UnRead
        {
            get
            {
                return Exists - Read;
            }
        }
    }
}
