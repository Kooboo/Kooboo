using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Response
{
    public class QRCodeResponse : IPaymentResponse
    {
        public QRCodeResponse(string codeurl, Guid requestId)
        {
            this.qrcode = codeurl;
            this.requestId = requestId;
        }

        [Description("The QR code content to scan and pay")]
        public string qrcode { get; set; }

        public Guid requestId { get; set; }

        [KIgnore]
        public string paymemtMethodReferenceId { get; set; }

        [Description("Generated sample HTML code to display the QR code")]
        public string html
        {
            get
            {
                return this.qrcodeHTML;
            }
        }

        [KIgnore]
        public string qrcodeHTML
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.qrcode))
                {
                    string html = @"<div id=""k-qrcode""></div> 
          <script type=""text/javascript"" src=""/_Admin/Scripts/lib/jquery.min.js""></script>
          <script type=""text/javascript"" src=""/_Admin/Scripts/lib/jquery.qrcode.min.js""></script>
          <script type=""text/javascript"">$('#k-qrcode').qrcode('" + this.qrcode + "')</script>";
                    return html;
                }
                return null;
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type => EnumResponseType.qrcode;
    }
}
