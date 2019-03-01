//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;
    using System.Collections.Concurrent;

    public class Pool<T, TProxy> : IInstanceProvider<T>, IDisposable
        where T : IDisposable
        where TProxy : T, IProxy<T>, new() {

        private Func<T> _createPoolItem;
        private ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();
        private bool _disposed = false;
        private object _lock = new object();

        public Pool(Func<T> createInstance) {
            _createPoolItem = createInstance;
        }

        public Pool(IInstanceProvider<T> provider)
            : this(provider.GetInstance) { }

        public T GetInstance() {
            T poolItem;

            if (!_pool.TryDequeue(out poolItem)) {
                poolItem = _createPoolItem();
            }

            return new TProxy() {
                WrappedItem = poolItem,
                OnDisposed = ReturnToPool,
            };
        }

        private bool ReturnToPool(T poolItem) {
            // Depopulate a disposed pool
            if (_disposed)
                return true;

            // Repopulate an empty pool
            if (_pool.IsEmpty) {
                _pool.Enqueue(poolItem);
                return false;
            }

            // Depopulate a full pool
            return true;
        }

        public void Dispose() {
            _disposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_pool != null) {
                    foreach (var item in _pool) {
                        item.Dispose();
                    }
                    _pool = null;
                }
            }
        }
    }
}
