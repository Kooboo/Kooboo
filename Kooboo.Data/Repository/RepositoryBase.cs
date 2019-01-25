//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.IndexedDB.Query;
using Kooboo.Data.Models;
using Kooboo.Events;

namespace Kooboo.Data.Repository
{
  public  class RepositoryBase<TValue> : IRepository<TValue> where TValue: IGolbalObject
    {
        private object _locker = new object();

        public  Database DatabaseDb
        {
             get
            {
              return  DB.Global(); 
            }
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
        public virtual bool  AddOrUpdate(TValue value)
        {
            var old = Store.get(value.Id);
            if (old == null)
            {
                Store.add(value.Id, value);
                RaiseEvent(value, ChangeType.Add, default(TValue));
                Store.Close(); 
                return true;
            }
            else
            {
                if (!IsEqual(old, value))
                {
                   
                    Store.update(value.Id, value);
                    RaiseEvent(value, ChangeType.Update, old);
                    Store.Close(); 
                    return true;
                }
            }

            Store.Close(); 
            return false;
        }

        /// <summary>
        /// Delete the item. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enableLog">Only some store has log enabled. This is used to disable log per action.</param>
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
                RaiseEvent(value, ChangeType.Delete,value);
                Store.Close(); 
            }
        }

        public virtual TValue  Get(Guid id, bool getColumnDataOnly = false)
        {
            if (getColumnDataOnly)
            {
                return Store.GetFromColumns(id);
            }
            else
            {
                return Store.get(id);
            }
        }
        

        public WhereFilter<Guid, TValue> Query
        {
            get
            {
                return new WhereFilter<Guid, TValue>(this.Store);
             
            }
        }

        public int Count()
        {
            return this.Store.Count();
        }

        public FullScan<Guid, TValue> TableScan
        {
            get
            {   return new FullScan<Guid, TValue>(this.Store);   }
        }

        /// <summary>
        /// Query all items out..... caution with performance.
        /// </summary>
        /// <param name="UseColumnData"></param>
        /// <returns></returns>
        public virtual List<TValue> All(bool UseColumnData = false)
        {
            if (UseColumnData)
            {
                return this.Store.Filter.UseColumnData().SelectAll();
            }
            else
            {
                return this.Store.Filter.SelectAll();
            }
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

            Type ValueType = value.GetType(); 

            if (ValueType == typeof(Binding))
            {
                Binding binding = value as Binding;
                var bindingchange = new BindingChange() { ChangeType = changetype, binding = binding }; 
                if (changetype == ChangeType.Update)
                {
                    bindingchange.OldBinding = oldvalue as Binding; 
                } 
                Events.EventBus.Raise(bindingchange);  
            }
        
            else if (ValueType == typeof(WebSite))
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
