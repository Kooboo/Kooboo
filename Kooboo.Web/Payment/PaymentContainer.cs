using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment
{
    public static class PaymentContainer
    {
        private static object _locker = new object();


        public static List<IPaymentRequestStore> _requestStore;

        [Obsolete]
        public static List<IPaymentRequestStore> RequestStore
        {
            get
            {
                if (_requestStore == null)
                {
                    lock (_locker)
                    {
                        if (_requestStore == null)
                        {
                            _requestStore = new List<IPaymentRequestStore>();

                            var alldefinedTypes = AssemblyLoader.LoadTypeByInterface(typeof(IPaymentRequestStore));
                            foreach (var item in alldefinedTypes)
                            {
                                var instance = Activator.CreateInstance(item) as IPaymentRequestStore;
                                _requestStore.Add(instance);
                            }
                        }
                    }
                }
                return _requestStore;
            }
        }

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
