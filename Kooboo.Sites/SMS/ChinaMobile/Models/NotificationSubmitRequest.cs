using System;
using System.Security.Cryptography;
using System.Text;

namespace Kooboo.Sites.SMS.ChinaMobile.Models
{
    /// <summary>
    /// Request model for CMCC Clout MAS API => /sms/norsubmit
    /// </summary>
    public class NotificationSubmitRequest
    {
        public NotificationSubmitRequest(
             string ecName,
             string apId,
             string secretKey,
             string mobiles,
             string content,
             string sign)
        {
            EcName = ecName;
            ApId = apId;
            Mobiles = mobiles;
            Content = content;
            Sign = sign;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(EcName);
            stringBuilder.Append(ApId);
            stringBuilder.Append(secretKey);
            stringBuilder.Append(Mobiles);
            stringBuilder.Append(Content);
            stringBuilder.Append(Sign);
            stringBuilder.Append(AddSerial);


            using (var md5CryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                byte[] md5Hash = md5CryptoServiceProvider.ComputeHash(inputBytes);

                Mac = BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
            };
        }

        /// <summary>
        /// EC company name of CMCC Cloud MAS account
        /// </summary>
        public string EcName { get; }

        /// <summary>
        /// ApId of SMS Interface created in CMCC Cloud MAS
        /// </summary>
        public string ApId { get; }

        /// <summary>
        /// Mobile phone numbers, separate by ','
        /// </summary>
        public string Mobiles { get; }

        /// <summary>
        /// SMS Content in Json format
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Signature of SMS Interface created in CMCC Cloud MAS
        /// </summary>
        public string Sign { get; }

        public string AddSerial { get; } = string.Empty;

        /// <summary>
        /// Join all the fields without delimiter, and in lowercase, and then encrypt to MD5
        /// </summary>
        public string Mac { get; }
    }
}
