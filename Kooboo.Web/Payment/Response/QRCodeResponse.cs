using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Web.Payment.Response;

namespace Kooboo.Web.Payment.Models
{
    public class QRCodeResponse : IPaymentResponse
    {
        public QRCodeResponse(string codeurl, Guid requestId)
        {
            this.qrcode = codeurl;
            this.ActionRequired = true;
            this.PaymentRequestId = requestId;
            this.Type = EnumResponseType.QrCode;
        }

        public bool ActionRequired { get;  set; }

        public string qrcode { get; set; }

        public Guid PaymentRequestId { get; set; }
        public string PaymemtMethodReferenceId { get; set; }

        public string html
        {
            get
            {
                return this.qrcodeHTML; 
            }
        }
         
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

        public EnumResponseType Type { get; set; }
    }
}
