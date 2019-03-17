using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Payment.Models
{
    public class QRCodeResponse : IPaymentResponse
    {
        public QRCodeResponse(string codeurl, Guid requestId)
        {
            this.QRCode = codeurl;
            this.ActionRequired = true;
            this.PaymentRequestId = requestId; 
        }

        public bool ActionRequired { get;  set; }

        public string QRCode { get; set; }
        public Guid PaymentRequestId { get; set; }
        public string PaymentReferenceId { get; set; }

        public string QRCodeHtml
        {
            get
            { 
                if (!string.IsNullOrWhiteSpace(this.QRCode))
                {
                    string html = "<div id=k-qrcode></div>";
                    html += "<script type='text/javascript' src='/_Admin/Scripts/lib/jquery.qrcode.min.js'></script>";

                    html += "<script type='text/javascript'>new QRCode(document.getElementById('k-qrcode'), \""+this.QRCode+"\");</script>";

                    return html; 

                }
                return null; 
            }
        }
    }
}
