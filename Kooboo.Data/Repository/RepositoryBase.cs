//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Events;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Query;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Repository
{
    public class RepositoryBase<TValue> : IRepository<TValue> where TValue : IGolbalObject
    {
        private static object _locker = new object();

        public Database DatabaseDb => DB.Global();

        protected virtual string StoreName => typeof(TValue).Name;

        protected virtual ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                return paras;
            }
        }

        private ObjectStore<Guid, TValue> _store;

        public ObjectStore<Guid, TValue> Store
        {
            get
            {
                if (_store == null)
                {
                    lock (_locker)
                    {
                        if (_store == null)
                        {
                            var para = StoreParameters;
                            para.SetPrimaryKeyField<TValue>(o => o.Id);

                            _store = DatabaseDb.GetOrCreateObjectStore<Guid, TValue>(StoreName, para);

                            if (!_store.CheckSameSetting(para))
                            {
                                _store = DatabaseDb.RebuildObjectStore<Guid, TValue>(_store, para);
                            }
                        }
                    }
                }
                return _store;
            }
        }

        public virtual bool AddOrUpdate(TValue value)
        {
            var old = Store.get(value.Id);
            if (old == null)
            {
                Store.add(value.Id, value);
                RaiseEvent(value, ChangeType.Add, default(TValue));
                Store.Close();
                return true;
            }

            if (!IsEqual(old, value))
            {
                Store.update(value.Id, value);
                RaiseEvent(value, ChangeType.Update, old);
                Store.Close();
                return true;
            }

            Store.Close();
            return false;
        }

        /// <summary>
        /// Delete the item.
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(Guid id)
        {
            var old = Get(id);
            if (old != null)
            {
                Store.delete(id);
                RaiseEvent(old, ChangeType.Delete, old);
                Store.Close();
            }
        }

        public virtual void Delete(TValue value)
        {
            if (value != null)
            {
                Store.delete(value.Id);
                RaiseEvent(value, ChangeType.Delete, value);
                Store.Close();
            }
        }

        public virtual TValue Get(Guid id, bool getColumnDataOnly = false)
        {
            if (getColumnDataOnly)
            {
                return Store.GetFromColumns(id);
            }

            return Store.get(id);
        }

        public WhereFilter<Guid, TValue> Query => new WhereFilter<Guid, TValue>(this.Store);

        public int Count()
        {
            return this.Store.Count();
        }

        public FullScan<Guid, TValue> TableScan => new FullScan<Guid, TValue>(this.Store);

        /// <summary>
        /// Query all items out..... caution with performance.
        /// </summary>
        /// <param name="useColumnData"></param>
        /// <returns></returns>
        public virtual List<TValue> All(bool useColumnData = false)
        {
            return useColumnData ? this.Store.Filter.UseColumnData().SelectAll() : this.Store.Filter.SelectAll();
        }

        public virtual bool IsEqual(TValue x, TValue y)
        {
            if (x.Id != y.Id)
            { return false; }

            return x.GetHashCode() == y.GetHashCode();
        }

        public virtual void RollBack(LogEntry log)
        {
            Store.RollBack(log);
        }

        public virtual void RollBack(List<LogEntry> loglist)
        {
            Store.RollBack(loglist);
        }

        public virtual void CheckOut(List<LogEntry> logs, ObjectStore<Guid, TValue> destinationStore)
        {
            Store.CheckOut(logs, destinationStore);
        }

        protected void RaiseEvent(TValue value, ChangeType changetype, TValue oldvalue)
        {
            if (value == null)
            { return; }

            //  Topic.Publish(TopicKeys.SystemInfoChanged);

            Type valueType = value.GetType();

            if (valueType == typeof(Binding))
            {
                Binding binding = value as Binding;
                var bindingchange = new BindingChange { ChangeType = changetype, Binding = binding };
                if (changetype == ChangeType.Update)
                {
                    bindingchange.OldBinding = oldvalue as Binding;
                }
                Events.EventBus.Raise(bindingchange);
            }
            else if (valueType == typeof(WebSite))
            {
                WebSite website = value as WebSite;

                var websitechange = new WebSiteChange() { ChangeType = changetype, WebSite = website };
                if (changetype == ChangeType.Update)
                {
                    websitechange.OldWebSite = oldvalue as WebSite;
                }
                Events.EventBus.Raise(websitechange);
            }
        }
    }
}