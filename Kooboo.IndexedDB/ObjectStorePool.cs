//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB
{
    public class ObjectStorePool<TKey, TValue>
    {
        private System.Collections.Queue myqueue = new System.Collections.Queue();
        private string storeName;
        private Database database;
        private ObjectStoreParameters parameters;

        private object _lock = new object();

        public ObjectStorePool(Database Database, string StoreName, ObjectStoreParameters Parameters)
        {
            this.database = Database;
            this.storeName = StoreName;
            if (parameters != null)
            {
                this.parameters = Parameters;
            }
            else
            {
                this.parameters = new ObjectStoreParameters();
            }
        }

        public ObjectStore<TKey, TValue> Current
        {
            get
            {
                lock (_lock)
                {
                    ObjectStore<TKey, TValue> store;

                    if (myqueue.Count > 0)
                    {
                        store = myqueue.Dequeue() as ObjectStore<TKey, TValue>;
                    }
                    else
                    {
                        store = new ObjectStore<TKey, TValue>(storeName, database, parameters);
                    }

                    return store;
                }
            }
        }

        public void ReleaseObject(ObjectStore<TKey, TValue> UsedStore)
        {
            lock (_lock)
            {
                if (UsedStore != null)
                {
                    myqueue.Enqueue(UsedStore);
                }
            }
        }

    }



}
