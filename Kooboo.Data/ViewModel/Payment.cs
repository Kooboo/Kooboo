//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.ViewModel
{

    public class PaymentRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }

        public int Years { get; set; }

        public Guid OrderId { get; set; }

        public string Payment { get; set; }

        public string UserIp { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// TwoCheckout token
        /// </summary>
        public string Token { get; set; }

        public BillAddress BillAddress { get; set; }

        public string PaypalReturnUrl { get; set; }

    }

    public class RechargeRequest
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public string UserIp { get; set; }

        [Required]
        public decimal Money { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        /// <summary>
        /// TwoCheckout token
        /// </summary>
        public string Token { get; set; }

        public BillAddress BillAddress { get; set; }

        public string PaypalReturnUrl { get; set; }
    }

    public class PaymentResponse
    {
        public Guid OrderId { get; set; }

        public Guid PaymentId { get; set; }

        public string Qrcode { get; set; }
        
        public string ApprovalUrl { get; set; }

        public string ErrorMessage { get; set; }

        public bool Paid { get; set; }

        public bool Success { get; set; }
    }

    public class PaymentStatusResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public enum PaymentMethod
    {
        Balance,
        Wechat,
        CreditCard,
        Paypal,
    }
    public class BillAddress
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
