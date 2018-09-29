//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Query;
using System.Collections.Generic;

namespace Kooboo.Mail.Repositories
{
    public abstract class RepositoryBase<TValue> : IRepository<TValue> where
        TValue : class, IMailObject
    {
        private object _locker = new object();

        public RepositoryBase(Database db)
        {
            Db = db;
        }

        protected Database Db
        {
            get;
            private set;
        }

        protected virtual string StoreName
        {
            get
            {
                return typeof(TValue).Name;
            }
        }

        protected virtual ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                return paras;
            }
        }

        private object _storeCreateLock = new object();
        private ObjectStore<int, TValue> _store;
        public ObjectStore<int, TValue> Store
        {
            get
            {
                if (_store == null)
                {
                    lock (_storeCreateLock)
                    {
                        if (_store == null)
                        {
                            var para = StoreParameters;
                            para.EnableLog = false;
                            para.EnableVersion = false;
                            para.SetPrimaryKeyField<TValue>(o => o.Id);
                            _store = Db.GetOrCreateObjectStore<int, TValue>(StoreName, para);
                        }
                    }
                }
                return _store;
            }
        }

        public virtual bool Add(TValue value)
        {
            bool ok = false; 
            if (value.Id == 0)
            {
                lock (_locker)
                {
                    value.Id = this.Store.LastKey + 1;
                    ok =  Store.add(value.Id, value);
                }
            }
            else
            {
                ok =  Store.add(value.Id, value);
            }
            Store.Close(); 
            return ok;
        }

        public virtual bool Update(TValue value)
        {
            Store.update(value.Id, value);
            Store.Close(); 
            return true;
        }

        public virtual bool AddOrUpdate(TValue value)
        {
            lock (_locker)
            {
                var old = Get(value.Id);
                if (old == null)
                {
                    if (value.Id == default(int))
                    {
                        value.Id = this.Store.LastKey + 1;
                    }
                    var ok = Store.add(value.Id, value);
                    Store.Close();
                    return ok;
                }
                else
                {
                    if (old.GetHashCode() != value.GetHashCode())
                    {
                        Store.update(value.Id, value);        
                        Store.Close(); 
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Delete the item. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enableLog">Only some store has log enabled. This is used to disable log per action.</param>
        public virtual bool Delete(int id)
        {
            Store.delete(id);
            Store.Close();
            return true;
        }

        public virtual TValue Get(int id)
        {
            return Store.get(id);
        }

        public int Count()
        {
            return this.Store.Count();
        }

        /// <summary>
        /// Query all items out..... caution with performance.
        /// </summary>
        /// <param name="useColumnData"></param>
        /// <returns></returns>
        public virtual List<TValue> All(bool useColumnData = false)
        {
            if (useColumnData)
            {
                return this.Store.Filter.UseColumnData().SelectAll();
            }
            else
            {
                return this.Store.Filter.SelectAll();
            }
        }

        internal FullScan<int, TValue> TableScan()
        {
            return new FullScan<int, TValue>(Store);
        }

        public WhereFilter<int, TValue> Query()
        {
            return new WhereFilter<int, TValue>(Store);
        }

    }
}
