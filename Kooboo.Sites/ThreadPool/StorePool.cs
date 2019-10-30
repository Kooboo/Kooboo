//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.Sites.Repository;
using System;
using System.Collections;

namespace Kooboo.Sites.ThreadPool
{
    public class StorePool<TValue> where TValue : class, ISiteObject
    {
        private Queue myqueue;

        private SiteRepositoryBase<TValue> repository;

        public object Locker = new object();

        public StorePool(SiteRepositoryBase<TValue> repository)
        {
            this.myqueue = new Queue();
            this.repository = repository;
        }

        public TValue Get(Guid id)
        {
            var store = GetAvailable();
            var siteobject = store.get(id);
            store.Close();
            ReleaseObject(store);
            return siteobject;
        }

        private ObjectStore<Guid, TValue> GetAvailable()
        {
            lock (Locker)
            {
                ObjectStore<Guid, TValue> store;

                if (myqueue.Count > 0)
                {
                    store = myqueue.Dequeue() as ObjectStore<Guid, TValue>;
                }
                else
                {
                    repository.Store.Close();
                    store = repository.SiteDb.DatabaseDb.GetReadingStore<Guid, TValue>(repository.StoreName, repository.StoreParameters);
                }
                return store;
            }
        }

        private void ReleaseObject(ObjectStore<Guid, TValue> usedStore)
        {
            lock (Locker)
            {
                if (usedStore != null)
                {
                    myqueue.Enqueue(usedStore);
                }
            }
        }

        public void ClearAll()
        {
            lock (Locker)
            {
                while (myqueue.Count > 0)
                {
                    var store = myqueue.Dequeue() as ObjectStore<Guid, TValue>;
                    store?.Close();
                    store = null;
                }
            }
        }
    }
}