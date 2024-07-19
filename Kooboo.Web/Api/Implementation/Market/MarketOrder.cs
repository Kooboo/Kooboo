global using System;
global using System.Collections.Generic;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Data.Models.CreditCard;
using Kooboo.Data.Models.Market;
using Kooboo.Sites.Payment;
using Microsoft.Extensions.Caching.Memory;


namespace Kooboo.Web.Api.Implementation.Market
{
    public class MarketOrderApi : IApi
    {
        public string ModelName => "MarketOrder";

        public bool RequireSite => false;

        public bool RequireUser => true;

        private void EnsureAdmin(ApiCall call)
        {
            if (!call.Context.User.IsAdmin)
            {
                throw new System.Exception("Only admin user of current organization are allowed to order");
            }
        }

        //public bool Paid(Guid OrderId, ApiCall call)
        public Kooboo.Api.ApiResponse.IResponse PaymentReturn(Guid id, ApiCall call)
        {
            // when payment from credit card back.
            var order = GetOrder(id);

            var url = Data.Helper.AccountUrlHelper.Order("Paid");

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("OrderId", id.ToString());

            var paidok = Lib.Helper.HttpHelper.Post<bool>(url, para);

            call.Context.Response.Redirect(302, "/_Admin/kconsole/consoleOrder");
            return new MetaResponse() { StatusCode = 302, RedirectUrl = "/_Admin/kconsole/consoleOrder" };

            // if does not redirect. 
            //            string html = @"<html><head> <meta http-equiv='refresh' content='2'; url='/_Admin/kdomain/domainOrders'></head>

            //<body>Redirecting to order page.</body></html>";

            //            return new PlainResponse()
            //            {
            //                Content = html
            //            };
        }

        private string GetPaymentCallBackUrl(Guid OrderId, ApiCall call)
        {
            return "https://" + call.Context.Request.Host + "/_api/MarketOrder/PaymentReturn?id=" + OrderId.ToString().Replace("-", "");
        }

        private Guid GetOrderId(OrderResponse res)
        {

            if (res != null && res.OrderId != default(Guid))
            {
                return res.OrderId;
            }

            if (res != null && res.IsSuccess == false && !string.IsNullOrEmpty(res.Message))
            {
                throw new Exception(res.Message);
            }

            throw new Exception("Order failed");

        }


        public System.Guid NewDomain(Data.ViewModel.Market.DomainOrderViewModel domainOrder, ApiCall call)
        {
            EnsureAdmin(call);
            domainOrder.OrganizationId = call.Context.User.CurrentOrgId;

            if (string.IsNullOrEmpty(domainOrder.Currency))
            {
                var org = Kooboo.Data.GlobalDb.Organization.Get(domainOrder.OrganizationId);
                domainOrder.Currency = org.Currency;
            }

            string json = System.Text.Json.JsonSerializer.Serialize(domainOrder);

            var url = Data.Helper.AccountUrlHelper.Order("Domain");

            var res = Lib.Helper.HttpHelper.Post<OrderResponse>(
                url,
                json,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            return GetOrderId(res);
        }

        //    public OrderResponse Topup(double Amount, string title, Guid OrgId, ApiCall call)
        public Guid NewTopUp(double Amount, ApiCall call)
        {
            if (Amount < 10)
            {
                var error = Hardcoded.GetValue("Amount must be at least 10", call.Context);
                throw new Exception(error);
            }

            var url = Data.Helper.AccountUrlHelper.Order("Topup");

            string title = Hardcoded.GetValue("Topup Balance");

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Amount", Amount);
            data.Add("title", title);
            var json = System.Text.Json.JsonSerializer.Serialize(data);

            var res = Lib.Helper.HttpHelper.Post<OrderResponse>(
                url,
                json,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            return GetOrderId(res);
        }

        public System.Guid NewMemberShip(int servicelevel, ApiCall call)
        {
            if (servicelevel == 0)
            {
                throw new Exception("Free product, purchase not needed");
            }

            var url = Data.Helper.AccountUrlHelper.Order("MemberShip");

            url += "?OrgId=" + call.Context.User.CurrentOrgId;

            url += "&ServiceLevel=" + servicelevel.ToString();

            var res = Lib.Helper.HttpHelper.Get<OrderResponse>(url);

            if (res != null)
            {
                if (res.IsSuccess)
                {
                    return res.OrderId;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(res.Message))
                    {
                        throw new System.Exception(res.Message);
                    }
                }
            }

            return System.Guid.Empty;

        }
        public IEnumerable<MarketOrder> RecentList(ApiCall call)
        {
            EnsureAdmin(call);

            var url = Data.Helper.AccountUrlHelper.Order("RecentList");

            Dictionary<string, string> para = new Dictionary<string, string>();

            return Lib.Helper.HttpHelper.Post<List<MarketOrder>>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }

        public ChargeResponse WeChatPay(ApiCall call, Guid id)
        {
            var url = Data.Helper.AccountUrlHelper.Order("WeChatPay");
            var para = new Dictionary<string, string>
            {
                { "orderId", id.ToString() },
            };

            var qrcode = Lib.Helper.HttpHelper.Post<string>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            return new ChargeResponse
            {
                RequestId = id,
                Paid = false,
                PaymentMethodReferenceId = id.ToString(),
                NextAction = new NextAction
                {
                    Type = NextAction.QRCODE,
                    ResponseData = qrcode,
                }
            };
        }

        public PaymentStatusResponse CheckStatus(ApiCall call, Guid id)
        {
            var url = Data.Helper.AccountUrlHelper.Order("WeChatPayStatus");

            var para = new Dictionary<string, string>
            {
                { "orderId", id.ToString() }
            };

            var paymentStatus = Lib.Helper.HttpHelper.Post<PaymentStatus>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            return new PaymentStatusResponse
            {
                Status = paymentStatus
            };
        }

        private MemoryCache orderCache { get; set; } = new MemoryCache(new MemoryCacheOptions() { });

        private MarketOrder GetOrder(Guid OrderId)
        {
            if (orderCache.TryGetValue<MarketOrder>(OrderId, out MarketOrder value))
            {
                if (value == null || value.TotalAmount <= 0)
                {
                    throw new Exception("Order not available");
                }
                return value;
            }
            var url = Data.Helper.AccountUrlHelper.Order("GetOrder");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("OrderId", OrderId.ToString());

            var order = Lib.Helper.HttpHelper.Post<MarketOrder>(url, para);

            if (order == null || order.TotalAmount <= 0)
            {
                throw new Exception("Order not available");
            }
            else
            {
                orderCache.Set<MarketOrder>(OrderId, order, TimeSpan.FromHours(3));
            }
            return order;
        }

        //  public MarketOrder GetOrder(Guid OrderId, ApiCall call)
        public OrderInfo Info(Guid OrderId, ApiCall call)
        {
            var order = GetOrder(OrderId);

            OrderInfo info = new OrderInfo();

            if (order.Type == EnumOrderType.domain)
            {
                info.Title = Hardcoded.GetValue("Domain Order", call.Context);

                var items = System.Text.Json.JsonSerializer.Deserialize<List<DomainItem>>(order.Body);

                string summary = "<br/>" + Hardcoded.GetValue("Domain will be assigned our DNS servers", call.Context);

                summary += "<br/>" + Hardcoded.GetValue("Domain allows you to use for website and email without configuration required", call.Context);

                info.Summary = summary;

                info.Items = new List<OrderInfo.OrderItem>();

                foreach (var item in items)
                {
                    info.Items.Add(new OrderInfo.OrderItem() { Name = item.DomainName, Price = item.Year });
                }

            }
            else if (order.Type == EnumOrderType.membership)
            {
                info.Title = Hardcoded.GetValue("MemberShip Order", call.Context);

                string summary = null;  //Hardcoded.GetValue("Membership grant your exclusive access to series resources: ", call.Context);
                summary += "<br/>" + Hardcoded.GetValue("Extra space and sites", call.Context);
                summary += "<br/>" + Hardcoded.GetValue("Commercial license", call.Context);
                summary += "<br/>" + Hardcoded.GetValue("Exclusive access to member only applications in the market place", call.Context);

                info.Summary = summary;

                MemberShip member = System.Text.Json.JsonSerializer.Deserialize<MemberShip>(order.Body);

                string name = Hardcoded.GetValue("Membership", call.Context);

                if (!string.IsNullOrWhiteSpace(member.Name))
                {
                    name += ": " + Hardcoded.GetValue(member.Name, call.Context);
                }

                info.Items.Add(new OrderInfo.OrderItem() { Name = name, Price = 1 });
            }
            else if (order.Type == EnumOrderType.topup)
            {
                info.Title = Hardcoded.GetValue("Topup Order", call.Context);

                info.Summary = "<br/>" + Hardcoded.GetValue("Balance allow you to pay for Api Market Service", call.Context);

                string name = Hardcoded.GetValue("Topup", call.Context);

                info.Items.Add(new OrderInfo.OrderItem() { Name = name, Price = order.TotalAmount });
            }

            info.SubTotal = Math.Round(order.TotalAmount, 2);
            info.Currency = order.Currency;
            info.Tax = 0;

            if (info.Currency == null)
            {
                info.Currency = "CNY";
            }

            return info;

        }



        private string appendStaging(string accounturl)
        {
#if DEBUG
            accounturl += "?staging=kooboostagingtemp2023";
#endif
            return accounturl;

        }


        //return current card list. 
        //    public List<CCPaymentMethod> PaymentMethods(Guid OrgId, ApiCall call)
        public List<PaymentMethod> CreditCards(ApiCall call)
        {
            EnsureAdmin(call);


            var url = Data.Helper.AccountUrlHelper.Payment("PaymentMethods");
            url = appendStaging(url);

            Dictionary<string, string> para = new Dictionary<string, string>();

            var accMethods = Lib.Helper.HttpHelper.Post<List<CCPaymentMethod>>(
                url,
                para,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            List<PaymentMethod> methods = new List<PaymentMethod>();

            if (accMethods != null)
            {
                foreach (var item in accMethods)
                {
                    methods.Add(new PaymentMethod() { Id = item.Id, Display = item.Last4, IsDefault = item.IsDefault, Type = item.Type });
                }
            }

            return methods;
        }


        // public CreditCardPaymentResult ChargePaymentMethod(PaymentMethodRequest request, ApiCall call)
        public SubmitResponse SubmitPaymentMethod(string PaymentMethodId, Guid OrderId, ApiCall call)
        {
            var order = GetOrder(OrderId);

            if (order == null)
            {
                return new SubmitResponse() { ErrorMessage = Hardcoded.GetValue("Invalid Order", call.Context) };
            }

            var url = Data.Helper.AccountUrlHelper.Payment("ChargePaymentMethod");
            url = appendStaging(url);

            PaymentMethodRequest request = new()
            {
                Amount = order.TotalAmount,
                PaymentMethodId = PaymentMethodId,
                OrderId = order.Id,
                ReturnUrl = GetPaymentCallBackUrl(OrderId, call)
            };

            var json = System.Text.Json.JsonSerializer.Serialize(request);

            var res = Lib.Helper.HttpHelper.Post<CreditCardPaymentResult>(
                url,
                json, Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            if (res != null && res.RedirectUrl != null)
            {
                return new SubmitResponse() { redirectUrl = res.RedirectUrl };
            }
            else
            {
                if (res == null)
                {
                    return new SubmitResponse() { Success = false };
                }
                else
                {
                    return new SubmitResponse() { Success = res.Success, redirectUrl = res.RedirectUrl, ErrorMessage = res.Message };
                }
            }
        }

        // public CreditCardPaymentResult ChargeNewCard(CreditCardPaymentRequest request, ApiCall call)
        public SubmitResponse SubmitCreditCard(string name, string CardNumber, string Year, string Month, string CVC, Guid OrderId, ApiCall call)
        {
            var order = GetOrder(OrderId);

            if (order == null || order.OrganizationId == default(Guid))
            {
                return new SubmitResponse() { ErrorMessage = Hardcoded.GetValue("Invalid Order", call.Context) };
            }

            var org = GlobalDb.Organization.Get(order.OrganizationId);

            var url = Data.Helper.AccountUrlHelper.Payment("ChargeNewCard");

            url = appendStaging(url);

            CreditCardPaymentRequest request = new()
            {
                Amount = order.TotalAmount,
                Currency = org.Currency,
                CVC = CVC,
                Name = name,
                Number = CardNumber,
                ExpYear = Year,
                ExpMonth = Month,
                OrderId = order.Id,
                OrganizationId = org.Id,
                ReturnUrl = GetPaymentCallBackUrl(OrderId, call)
            };

            var json = System.Text.Json.JsonSerializer.Serialize(request);

            var res = Lib.Helper.HttpHelper.Post<CreditCardPaymentResult>(
                url,
                json,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );

            if (res != null && res.RedirectUrl != null)
            {
                return new SubmitResponse() { redirectUrl = res.RedirectUrl };
            }
            else
            {
                if (res == null)
                {
                    return new SubmitResponse() { Success = false };
                }
                else
                {
                    return new SubmitResponse() { Success = res.Success, redirectUrl = res.RedirectUrl, ErrorMessage = res.Message };
                }
            }
        }
    }


    public record PaymentMethod
    {

        public string Id { get; set; }

        public string Display { get; set; }

        public string Type { get; set; }  // VISA or Master.

        public bool IsDefault { get; set; }

    }


    public record SubmitResponse
    {
        public string redirectUrl { get; set; }

        public string ErrorMessage { get; set; }

        public bool Success { get; set; } = false;
    }

    public record OrderInfo
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public double SubTotal { get; set; }

        public double Tax { get; set; }

        public string Currency { get; set; }

        public double Total
        {
            get
            {
                return this.SubTotal + this.Tax;
            }
        }

        public record OrderItem
        {
            public string Name { get; set; }

            public double Price { get; set; }
        }
    }
}
