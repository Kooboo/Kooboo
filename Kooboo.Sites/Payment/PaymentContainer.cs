using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment
{
    public static class PaymentContainer
    {
        private static object _locker = new object();
          
        private static List<IPaymentMethod> _paymentmethods;

        public static List<IPaymentMethod> PaymentMethods
        {
            get
            {
                if (_paymentmethods == null)
                {
                    _paymentmethods = Lib.IOC.Service.GetInstances<IPaymentMethod>();
                }
                return _paymentmethods;
            } 
        }
    }


} 
