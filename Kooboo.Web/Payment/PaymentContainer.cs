using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Payment
{
    public static class PaymentContainer
    {
        private static object _locker = new object();

        private static List<IPaymentCallbackWorker> _callbackworkers;

        public static List<IPaymentCallbackWorker> CallBackWorkers
        {
            get
            {
                if (_callbackworkers == null)
                {
                    lock (_locker)
                    {
                        if (_callbackworkers == null)
                        {
                            _callbackworkers = new List<IPaymentCallbackWorker>();

                            var alldefinedTypes = AssemblyLoader.LoadTypeByInterface(typeof(IPaymentCallbackWorker));
                            foreach (var item in alldefinedTypes)
                            {
                                var instance = Activator.CreateInstance(item) as IPaymentCallbackWorker;
                                _callbackworkers.Add(instance);
                            }
                        }
                    }
                }
                return _callbackworkers;
            }
        }

        public static List<IPaymentRequestStore> _requestStore;

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
                    lock (_locker)
                    {
                        if (_paymentmethods == null)
                        {
                            _paymentmethods = new List<IPaymentMethod>();

                            var alltypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IPaymentMethod));

                            foreach (var item in alltypes)
                            {
                                var instance = Activator.CreateInstance(item) as IPaymentMethod;
                                _paymentmethods.Add(instance);
                            }
                        }
                    }
                }
                return _paymentmethods;
            }
        }
    }
}