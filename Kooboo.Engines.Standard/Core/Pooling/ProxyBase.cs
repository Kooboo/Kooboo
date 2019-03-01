//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;

    public class ProxyBase<T> : IProxy<T>
        where T : class, IDisposable {
        public T WrappedItem { get; set; }

        public ProxyBase() { }

        public ProxyBase(T wrapped) {
            WrappedItem = wrapped;
        }

        public Func<T, bool> OnDisposed { get; set; }

        public void Dispose() {
            if (OnDisposed == null || OnDisposed(WrappedItem)) {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (WrappedItem != null) {
                    WrappedItem.Dispose();
                    WrappedItem = null;
                }
            }
        }
    }
}
