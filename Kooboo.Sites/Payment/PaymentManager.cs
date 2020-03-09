using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Payment
{

    public static class PaymentManager
    {
        private static object _locker = new object();

        public static List<IPaymentMethod> GetCurrencyMethods(string currency, List<IPaymentMethod> methods)
        {
            List<IPaymentMethod> result = new List<IPaymentMethod>();

            foreach (var item in methods)
            {
                if (item.supportedCurrency != null && item.supportedCurrency.Any())
                {
                    if (item.supportedCurrency.Contains(currency, StringComparer.OrdinalIgnoreCase))
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add(item);
                }

            }

            return result;
        }

        public static List<IPaymentMethod> GetSiteAvailableMethods()
        {
            return PaymentContainer.PaymentMethods.Where(o => !(o is IKoobooPayment)).ToList();
        }

        public static IPaymentMethod GetMethod(string paymentmethod)
        {
            paymentmethod = paymentmethod.ToLower();

            foreach (var item in PaymentContainer.PaymentMethods)
            {
                if (item.Name.ToLower() == paymentmethod)
                {
                    return item;
                }
            }
            return null;
        }

        public static IPaymentMethod GetMethod(string MethodName, RenderContext context)
        {
            if (MethodName == null)
            {
                return null;
            }

            var method = GetMethod(MethodName);

            if (method != null)
            {
                var methodType = method.GetType();

                var sitedb = context.WebSite.SiteDb();

                var paymentmethod = Activator.CreateInstance(methodType) as IPaymentMethod;
                paymentmethod.Context = context;

                if (Lib.Reflection.TypeHelper.HasGenericInterface(methodType, typeof(IPaymentMethod<>)))
                {
                    var settingtype = Lib.Reflection.TypeHelper.GetGenericType(methodType);

                    if (settingtype != null)
                    {
                        var settingvalue = sitedb.CoreSetting.GetSiteSetting(settingtype) as IPaymentSetting;
                        //Setting
                        var setter = Lib.Reflection.TypeHelper.GetSetObjectValue("Setting", methodType, settingtype);
                        setter(paymentmethod, settingvalue);

                        return paymentmethod;
                    }
                    else
                    {
                        throw new Exception(MethodName + " missing setting infomatoin");
                    }
                }

            }

            else

            {
                throw new Exception(MethodName + " not found");
            }

            throw new Exception(MethodName + " missing setting infomatoin");
        }

        [Obsolete]
        public static IPaymentResponse MakePayment(PaymentRequest request, RenderContext context)
        {
            // TODO: remove this part.... Make the payment at certain location only...  
            //var method = GetMethod(request.PaymentMethod, context);
            //method.Context = context;

            //ValidateRequest(method, request, context);

            //// save the request data..  
            //var payment = method.Charge(request);

            //if (!string.IsNullOrEmpty(payment.paymemtMethodReferenceId))
            //{
            //    request.ReferenceId = payment.paymemtMethodReferenceId;

            //    SaveRequest(request, context);
            //}

            //if (payment.requestId == default(Guid))
            //{
            //    payment.requestId = request.Id;
            //}

            //if (payment is PaidResponse)
            //{
            //    CallBack(new PaymentCallback() { Status = PaymentStatus.Paid, PaymentRequestId = payment.requestId }, context);
            //    /////Kooboo.Server.Commerce.PaymentRequestService.Paid(payment.PaymentRequestId);
            //}
            //else if (payment is FailedResponse)
            //{
            //    CallBack(new PaymentCallback() { Status = PaymentStatus.Rejected, PaymentRequestId = payment.requestId }, context);
            //    //Kooboo.Server.Commerce.PaymentRequestService.Cancel(payment.PaymentRequestId);
            //} 
            //return payment;
            return null;
        }

        public static void ValidateRequest(IPaymentMethod paymentMethod, PaymentRequest request, RenderContext context)
        {

            if (request.PaymentMethod == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Payment Method not found", context));
            }

            if (paymentMethod.supportedCurrency != null && paymentMethod.supportedCurrency.Any())
            {
                if (paymentMethod.supportedCurrency.Count() == 1)
                {
                    if (string.IsNullOrWhiteSpace(request.Currency))
                    {
                        request.Currency = paymentMethod.supportedCurrency[0];
                    }
                    else
                    {
                        request.Currency = request.Currency.ToUpper();
                        if (request.Currency != paymentMethod.supportedCurrency[0])
                        {
                            throw new Exception(Data.Language.Hardcoded.GetValue("currency not supported by payment method", context));
                        }
                    }
                }
                else if (!paymentMethod.supportedCurrency.Contains(request.Currency, StringComparer.OrdinalIgnoreCase))
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("currency not supported by payment method", context));
                }
            }

            if (string.IsNullOrWhiteSpace(request.Currency))
            {
                request.Currency = "EUR";
            }
        }

        public static void CallBack(PaymentCallback callback, RenderContext context)
        {
            var workers = Lib.IOC.Service.GetInstances<IPaymentCallbackWorker>();
            foreach (var item in workers)
            {
                item.Process(callback, context);
            }
        }

        public static PaymentRequest GetRequest(Guid PaymentRequestId, RenderContext context)
        {
            if (context.WebSite != null)
            {
                var sitedb = context.WebSite.SiteDb();
                var repo = sitedb.GetSiteRepository<Repository.PaymentRequestRepository>();

                var result = repo.Get(PaymentRequestId);

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static PaymentStatusResponse EnquireStatus(PaymentRequest Request, RenderContext context)
        {
            var paymentmethod = PaymentManager.GetMethod(Request.PaymentMethod, context);
            if (paymentmethod != null)
            {
                paymentmethod.Context = context;

                var status = paymentmethod.checkStatus(Request);

                return status;
            }
            return new PaymentStatusResponse() { Message = "" };
        }

        public static void UpdateRequest(PaymentRequest request, RenderContext context)
        {
            if (context.WebSite != null)
            {
                var sitedb = context.WebSite.SiteDb();
                var repo = sitedb.GetSiteRepository<Repository.PaymentRequestRepository>();
                repo.AddOrUpdate(request);
            }
        }

        public static PaymentRequest GetRequestByReferece(string ReferenceId, RenderContext context)
        {
            if (context.WebSite != null)
            {
                var sitedb = context.WebSite.SiteDb();
                var repo = sitedb.GetSiteRepository<Repository.PaymentRequestRepository>();

                var hash = Lib.Security.Hash.ComputeHashGuid(ReferenceId); 

                var result = repo.Query.Where(o => o.ReferenceIdHash == hash).FirstOrDefault(); 

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }

}
