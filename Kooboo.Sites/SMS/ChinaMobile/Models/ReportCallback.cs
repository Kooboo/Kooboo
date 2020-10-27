namespace Kooboo.Sites.SMS.ChinaMobile.Models
{
    public class ReportCallback
    {
        /// <summary>
        /// Status code when succeed => DELIVRD
        /// </summary>
        public string ReportStatus { get; set; }

        /// <summary>
        /// Callback phonenumber, one at each time
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Submit date, format: yyyyMMddHHmmss
        /// </summary>
        public string SubmitDate { get; set; }

        /// <summary>
        /// Receive date, format: yyyyMMddHHmmss
        /// </summary>
        public string ReceiveDate { get; set; }

        /// <summary>
        /// Status code when failed, e.g. "DB:0140"
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Message group id, set when submitting SMS sending request
        /// </summary>
        public string MsgGroup { get; set; }
    }
}
