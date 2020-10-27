namespace Kooboo.Sites.SMS.ChinaMobile.Models
{
    public class SMSConfig
    {
        /// <summary>
        /// EC company name of CMCC Cloud MAS account
        /// </summary>
        public string EcName { get; set; }

        /// <summary>
        /// ApId of SMS Interface created in CMCC Cloud MAS
        /// </summary>
        public string ApId { get; set; }

        /// <summary>
        /// SecretKey of SMS Interface created in CMCC Cloud MAS
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Signature of SMS Interface created in CMCC Cloud MAS
        /// </summary>
        public string Sign { get; set; }
    }
}
