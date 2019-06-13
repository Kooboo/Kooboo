//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB;
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Query;
using Kooboo.Sites.Models;
using Kooboo.Sites.Routing;
using Kooboo.Events.Cms;
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Repository
{
    public class SiteRepositoryBase<TValue> : ISiteRepositoryBase, IRepository, IRepository<TValue> where TValue : class, ISiteObject
    {
        protected object _locker = new object();

        private SiteDb _sitedb;
        public virtual SiteDb SiteDb
        {
            get
            {
                if (_sitedb == null && WebSite != null)
                {
                    _sitedb = Kooboo.Sites.Cache.WebSiteCache.GetSiteDb(WebSite);
                }
                return _sitedb;
            }
            set
            {
                _sitedb = value;
            }
        }

        private Type _SiteObjectType;
        public Type SiteObjectType
        {
            get
            {
                if (_SiteObjectType == null)
                {
                    _SiteObjectType = typeof(TValue);
                }
                return _SiteObjectType;
            }
            set { _SiteObjectType = value; }
        }

        public bool UseCache { get; set; }

        private WebSite _website;
        public WebSite WebSite
        {
            get
            {
                if (_website == null && _sitedb != null)
                {
                    _website = this._sitedb.WebSite;
                }
                return _website;
            }
            set
            {
                _website = value;
            }
        }

        public void init()
        {

            this.UseCache = Cache.WebSiteCache.EnableCache(this.WebSite, this.SiteObjectType);

            if (this.UseCache)
            {
                foreach (var item in All())
                {
                    Kooboo.Sites.Cache.SiteObjectCache<TValue>.AddOrUpdate(this.SiteDb, item);
                }
            }
        }

        public SiteRepositoryBase(WebSite website)
        {
            this.WebSite = website;
            init();
        }
        public SiteRepositoryBase(SiteDb sitedb)
        {
            this.SiteDb = sitedb;
            this.WebSite = sitedb.WebSite;
            init();
        }

        public SiteRepositoryBase()
        {

        }

        public virtual bool IsEqualTo(TValue value)
        {
            return this.GetHashCode() == value.GetHashCode();
        }

        public virtual string StoreName
        {
            get
            {
                return typeof(TValue).Name;
            }
        }

        public virtual ObjectStoreParameters StoreParameters
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

                            var para = this.StoreParameters;

                            if (Lib.Reflection.TypeHelper.HasInterface(this.SiteObjectType, typeof(ICoreObject)))
                            {
                                para.EnableLog = true;
                                para.EnableVersion = true;
                                para.SetPrimaryKeyField<TValue>(o => o.Id);
                            }
                            else
                            {
                                para.EnableLog = false;
                                para.EnableVersion = false;
                            }

                            _store = this.SiteDb.DatabaseDb.GetOrCreateObjectStore<Guid, TValue>(StoreName, para);

                            if (_store.CheckSameSetting(para) == false)
                            {
                                _store = this.SiteDb.DatabaseDb.RebuildObjectStore<Guid, TValue>(_store, para);
                            }
                        }
                    }
                }
                return _store;
            }
        }
          
        public virtual bool AddOrUpdate(TValue value, Guid UserId)
        {
            lock (_locker)
            {
                var old = Get(value.Id);
                if (old == null)
                {
                    RaiseBeforeEvent(value, ChangeType.Add);
                    Store.CurrentUserId = UserId;
                    Store.add(value.Id, value);
                    RaiseEvent(value, ChangeType.Add);
                    return true;
                }
                else
                {
                    if (!IsEqual(old, value))
                    {
                        value.LastModified = DateTime.UtcNow;
                        RaiseBeforeEvent(value, ChangeType.Add);
                        Store.CurrentUserId = UserId;
                        Store.update(value.Id, value);
                        RaiseEvent(value, ChangeType.Update, old);
                        return true;
                    }
                }
                return false;
            }
        }

        public virtual bool AddOrUpdate(TValue value)
        {
            return this.AddOrUpdate(value, default(Guid));
        }

        public virtual void Delete(Guid id)
        {
            this.Delete(id, default(Guid));
        }

        public virtual void Delete(Guid id, Guid UserId)
        {
            lock (_locker)
            {
                var old = Get(id);
                if (old != null)
                {
                    RaiseBeforeEvent(old, ChangeType.Delete);
                    this.Store.CurrentUserId = UserId;
                    Store.delete(id);
                }
                RaiseEvent(old, ChangeType.Delete);
            }
        }

        public virtual TValue Get(Guid id, bool getColumnDataOnly = false)
        {
            TValue result;
            if (this.UseCache)
            {
                result = Cache.SiteObjectCache<TValue>.Get(this.SiteDb, id);
                if (result != null)
                {
                    return result;
                }
            }

            if (getColumnDataOnly)
            {
                result = Store.GetFromColumns(id);
            }
            else
            {
                result = Store.get(id);
            }

            if (result != null)
            {
                if (this.UseCache)
                {
                    Cache.SiteObjectCache<TValue>.AddOrUpdate(this.SiteDb, result);
                }
            }

            return result;
        }

        public virtual TValue GetFromCache(Guid id)
        {
            TValue result;
            if (this.UseCache)
            {
                result = Cache.SiteObjectCache<TValue>.Get(this.SiteDb, id);
                if (result != null)
                {
                    return result;
                }
            }
            return default(TValue);
        }

        public virtual TValue Get(string nameorid)
        {
            return this.GetByNameOrId(nameorid);
        }

        public virtual TValue GetWithEvent(Guid id)
        {
            if (this.WebSite.EnableFrontEvents)
            {
                // Fire the event here.. 
            }
            return Get(id);
        }

        public virtual TValue GetByUrl(string relativeUrl)
        {
            string relativeurl = UrlHelper.RelativePath(relativeUrl);

            Route route = this.SiteDb.Routes.GetByUrl(relativeUrl);

            if (route != null)
            {
                var routetype = Service.ConstTypeService.GetModelType(route.DestinationConstType);

                if (routetype == this.SiteObjectType)
                {
                    return Get(route.objectId);
                }
            }
            return default(TValue);
        }


        public virtual TValue GetByNameOrId(string NameOrGuid)
        {
            Guid key;
            bool parseok = Guid.TryParse(NameOrGuid, out key);

            if (!parseok)
            {
                byte consttype = ConstTypeContainer.GetConstType(typeof(TValue));

                key = Data.IDGenerator.Generate(NameOrGuid, consttype);
            }
            return Get(key);
        }

        public virtual List<UsedByRelation> GetUsedBy(Guid ObjectId)
        {
            //var siteojbect = this.Store.getValueFromColumns(ObjectId) as SiteObject;

            //if (siteojbect != null)
            //{
            //    var objectrelations = this.SiteDb.Relations.GetReferredBy(siteojbect);
            //    return Helper.RelationHelper.ShowUsedBy(this.SiteDb, objectrelations);
            //}

            var objectrelations = this.SiteDb.Relations.GetReferredBy(this.SiteObjectType, ObjectId);
            return Helper.RelationHelper.ShowUsedBy(this.SiteDb, objectrelations);

            // return new List<UsedByRelation>(); 
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
            {
                return new FullScan<Guid, TValue>(this.Store);
            }
        }

        /// <summary>
        /// Query all items out..... caution with performance.
        /// </summary>
        /// <param name="UseColumnData"></param>
        /// <returns></returns>
        public virtual List<TValue> All(bool UseColumnData)
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

        public virtual List<TValue> All()
        {
            return All(false);
        }

        /// <summary>
        /// get a list of the site object. will use cache first if enabled
        /// </summary>
        /// <returns></returns>
        public virtual List<TValue> List(bool UseColumnData = false)
        {
            if (this.UseCache)
            {
                return Cache.SiteObjectCache<TValue>.List(this.SiteDb);
            }
            else
            {
                return All();
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
            Guid key = this.Store.KeyConverter.FromByte(log.KeyBytes);

            if (log.EditType == EditType.Add)
            {
                var siteobject = this.Get(key);
                if (siteobject != null)
                {
                    if (this.SiteObjectType == typeof(Route))
                    {
                        var route = siteobject as Route;
                        if (route != null)
                        {
                            Service.LogService.EnsureDeleteRouteObject(this.SiteDb, route);
                        }
                    }

                    else if (Attributes.AttributeHelper.IsRoutable(siteobject))
                    {
                        Service.LogService.EnsureDeleteObjectRoute(this.SiteDb, siteobject as SiteObject);
                    }
                    Delete(key);
                }

            }
            else if (log.EditType == EditType.Update)
            {
                TValue oldvalue = this.Store.getValue(log.OldBlockPosition);
                AddOrUpdate(oldvalue);
            }
            else
            {
                // this is delete of one item. 
                TValue oldvalue = this.Store.getValue(log.OldBlockPosition);
                AddOrUpdate(oldvalue);
                if (this.SiteObjectType == typeof(Route))
                {
                    var route = oldvalue as Route;
                    if (route != null)
                    {
                        Service.LogService.EnsureRestoreRouteObject(this.SiteDb, route);
                    }
                }

                else if (Attributes.AttributeHelper.IsRoutable(oldvalue))
                {
                    Service.LogService.EnsureRestoreObjectRoute(this.SiteDb, oldvalue as SiteObject);
                }
            }
        }

        public virtual void RollBack(List<LogEntry> loglist)
        {
            HashSet<Guid> finished = new HashSet<Guid>();

            foreach (var item in loglist.OrderBy(o => o.TimeTick))
            {
                Guid key = this.Store.KeyConverter.FromByte(item.KeyBytes);

                if (!finished.Contains(key))
                {
                    RollBack(item);
                    finished.Add(key);
                }
            }
        }


        public virtual List<Relation.ObjectRelation> CheckBeingUsed(TValue SiteObject)
        {

            var converted = SiteObject as SiteObject;
            if (converted == null)
            {
                return null;
            }
            return this.SiteDb.Relations.GetReferredBy(converted);
        }

        public List<Relation.ObjectRelation> CheckBeingUsed(Guid ObjectId)
        {
            var siteobject = this.Get(ObjectId);
            return this.CheckBeingUsed(siteobject);
        }

        protected void RaiseBeforeEvent(TValue value, ChangeType changetype, TValue oldValue = default(TValue))
        {
            if (this.SiteObjectType == typeof(Page))
            {
                var maxpages = Kooboo.Data.Authorization.QuotaControl.MaxPages(this.SiteDb.WebSite.OrganizationId);
                if (maxpages != int.MaxValue)
                {
                    var count = this.SiteDb.Pages.Count();
                    if (count >= maxpages)
                    {
                        throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Max number of pages per site has been reached, service level upgrade required"));
                    }
                }
            }


            if (this.SiteDb.WebSite.EnableConstraintFixOnSave && changetype != ChangeType.Delete)
            {
                Constraints.ConstraintChecker.FixOnSave(this.SiteDb, value as SiteObject);
            }

            // for kscript parameters. 
            if (value is IScriptable)
            {
                var kscriptobject = value as IScriptable;
                var domobjct = value as IDomObject;
                if (kscriptobject != null && domobjct != null)
                {
                    kscriptobject.RequestParas = Kooboo.Sites.Scripting.ScriptHelper.GetkScriptParaFromDom(this.SiteDb, domobjct.Dom);
                }
            }

            if (value is Kooboo.Sites.Models.Code)
            {
                var code = value as Kooboo.Sites.Models.Code;

                if (code.CodeType == Sites.Models.CodeType.PageScript)
                {
                    code.Parameters = Kooboo.Sites.Scripting.ScriptHelper.GetKScriptParameters(code.Body);
                }
            }

            if (value is Kooboo.Sites.Models.CoreObject && changetype != ChangeType.Delete)
            {
                if (value is Kooboo.Sites.Routing.Route)
                {
                    return;
                }

                var size = Kooboo.Sites.Service.ObjectService.GetSize(value);

                if (!Kooboo.Data.Infrastructure.InfraManager.Test(this.WebSite.OrganizationId, Data.Infrastructure.InfraType.Disk, size))
                {
                    var message = Data.Language.Hardcoded.GetValue("Over Disk Quota");
                    throw new Exception(message);
                }
                else
                {

                    string msg = ConstTypeContainer.GetName(value.ConstType);

                    var objinfo = Kooboo.Sites.Service.ObjectService.GetObjectInfo(this.SiteDb, value);


                    if (objinfo != null)
                    {
                        msg += "| " + objinfo.DisplayName;
                    }
                    else
                    {
                        msg += "| " + value.Name;
                    }

                    Kooboo.Data.Infrastructure.InfraManager.Add(this.WebSite.OrganizationId, Data.Infrastructure.InfraType.Disk, size, msg);

                }

            }

        }

        /// <summary>
        /// After event
        /// </summary>
        /// <param name="value"></param>
        /// <param name="changetype"></param>
        /// <param name="oldValue"></param>
        protected void RaiseEvent(TValue value, ChangeType changetype, TValue oldValue = default(TValue))
        {
            if (value == null)
            { return; }

            if (changetype == ChangeType.Delete && value is ICoreObject)
            {
                var bytes = Service.ObjectService.KeyConverter.ToByte(value.Id);
                var lastdeletelogid = SiteDb.Log.GetJustDeletedVersion(this.StoreName, bytes);
                var core = value as ICoreObject;
                core.Version = lastdeletelogid;
            }

            var siteevent = new SiteObjectEvent<TValue>
            {
                Value = value,
                ChangeType = changetype,
                SiteDb = SiteDb,
                Store = this.Store
            }; 


            if (changetype == ChangeType.Update)
            {
                siteevent.OldValue = oldValue;
            }

            if (this.SiteDb.WebSite.EnableDiskSync)
            {
                Sync.DiskSyncHelper.SyncToDisk(SiteDb, value, changetype, this.StoreName);
            }



            if (this.SiteDb.WebSite.EnableFullTextSearch)
            {
                this.SiteDb.SearchIndex.Sync(SiteDb, value, changetype, this.StoreName);
            }

            if (this.SiteDb.WebSite.EnableCluster && this.SiteDb.ClusterManager != null)
            {
                this.SiteDb.ClusterManager.AddTask(this, value, changetype);
            }

            ///add to cache. 
            if (this.UseCache)
            {
                if (changetype == ChangeType.Delete)
                {
                    Cache.SiteObjectCache<TValue>.Remove(this.SiteDb, value);
                }
                else
                {
                    Cache.SiteObjectCache<TValue>.AddOrUpdate(this.SiteDb, value);
                }
            }

            if (siteevent.ChangeType == ChangeType.Delete)
            {
                var objectvalue = siteevent.Value;
                if (Attributes.AttributeHelper.IsRoutable(objectvalue))
                {
                    Sites.Helper.ChangeHelper.DeleteRoutableObject(SiteDb, this, objectvalue);
                }
                Sites.Helper.ChangeHelper.DeleteComponentFromSource(SiteDb, objectvalue); 
            }

            Relation.RelationManager.Compute(siteevent);

            Kooboo.Sites.Events.Handler.HandleChange(siteevent);

            Data.Events.EventBus.Raise(siteevent);

            if (siteevent.Value is DomObject)
            {
                var newojbect = siteevent.Value as DomObject;
                newojbect.DisposeDom();
            }

            if (siteevent.Value is Page)
            {
                PageRoute.UpdatePageRouteParameter(siteevent.SiteDb, siteevent.Value.Id);
            }

            ///close database
            if (value is ICoreObject)
            {
                this.Store.Close();
                if (this.SiteObjectType == typeof(Image))
                {
                    lock (this.SiteDb.ImagePool.Locker)
                    {
                        this.SiteDb.ImagePool.ClearAll();
                    }
                }
            }
            else
            {

                //{
                //    this.Store.Close();
                //    if (this.SiteObjectType == typeof(Image))
                //    {
                //        lock (this.SiteDb.ImagePool.Locker)
                //        {
                //            this.SiteDb.ImagePool.ClearAll();
                //        }
                //    }
                //}

            }

        }



        #region NonGeneric

        bool IRepository.AddOrUpdate(Object value, Guid UserId)
        {
            var tvalue = (TValue)value;

            return this.AddOrUpdate(tvalue, UserId);
        }

        void IRepository.RollBack(LogEntry log)
        {
            this.RollBack(log);
        }

        void IRepository.RollBack(List<LogEntry> loglist)
        {
            this.RollBack(loglist);
        }

        void IRepository.Delete(Guid id, Guid UserId = default(Guid))
        {
            this.Delete(id, UserId);
        }

        ISiteObject IRepository.Get(Guid id, bool getColumnDataOnly)
        {
            return this.Get(id, getColumnDataOnly);
        }

        ISiteObject IRepository.GetByNameOrId(string NameOrId)
        {
            return this.GetByNameOrId(NameOrId);
        }

        ISiteObject IRepository.GetByLog(LogEntry log)
        {
            long blockposition = 0;
            if (log.EditType == EditType.Delete)
            {
                blockposition = log.OldBlockPosition;
            }
            else
            {
                blockposition = log.NewBlockPosition;
            }

            var result = this.Store.getValue(blockposition);

            if (result is ICoreObject)
            {
                // in case it is an delete. 
                var core = result as ICoreObject;
                if (core != null)
                {
                    core.Version = log.Id;
                }
            }
            return result;
        }

        ISiteObject IRepository.GetLastEntryFromLog(Guid ObjectId)
        {
            if (ObjectId == default(Guid))
            { return null; }
            var result = this.Get(ObjectId);
            if (result != null)
            {
                return result;
            }

            var keybytes = this.Store.KeyConverter.ToByte(ObjectId);

            var logentry = this.SiteDb.Log.GetLastLogByStoreNameAndKey(this.StoreName, keybytes);

            if (logentry != null)
            {
                long blockposition = 0;
                if (logentry.EditType == EditType.Delete)
                {
                    blockposition = logentry.OldBlockPosition;
                }
                else
                {
                    blockposition = logentry.NewBlockPosition;
                }

                return this.Store.getValue(blockposition);
            }

            return null;
        }

        List<ISiteObject> IRepository.All(bool UseColumnData)
        {
            var list = this.All(UseColumnData);
            return list.Select(it => (ISiteObject)it).ToList();
        }


        Type IRepository.ModelType
        {
            get { return this.SiteObjectType; }
            set { this.SiteObjectType = value; }
        }

        string IRepository.StoreName
        {
            get
            {
                return this.StoreName;
            }
        }

        IObjectStore IRepository.Store
        {
            get
            {
                return this.Store as IObjectStore;
            }
        }

        void IRepository.CheckOut(Int64 VersionId, IRepository DestinationRepository, bool SelfIncluded)
        {
            List<LogEntry> logs;
            int namehash = this.StoreName.GetHashCode32();
            if (SelfIncluded)
            {
                logs = this.SiteDb.Log.Store.Where(o => o.Id > VersionId && o.StoreNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.SiteDb.Log.Store.Where(o => o.Id >= VersionId && o.StoreNameHash == namehash).Take(99999);
            }

            CheckOutExcl(logs, DestinationRepository);

        }

        List<UsedByRelation> IRepository.GetUsedBy(Guid ObjectId)
        {
            return this.GetUsedBy(ObjectId);
        }

        internal void CheckOut(List<LogEntry> logs, IRepository DestinationRepository)
        {
            HashSet<Guid> donekeys = new HashSet<Guid>();
            foreach (var item in logs.OrderByDescending(o => o.TimeTick))
            {
                Guid key = this.Store.KeyConverter.FromByte(item.KeyBytes);

                if (donekeys.Contains(key))
                {
                    continue;
                }

                if (item.EditType == EditType.Add || item.EditType == EditType.Update)
                {
                    TValue value = this.Store.getValue(item.NewBlockPosition);
                    DestinationRepository.AddOrUpdate(value);

                    donekeys.Add(key);
                }
                else
                {
                    donekeys.Add(key);
                }
            }
        }

        internal void CheckOutExcl(List<LogEntry> ExclLogs, IRepository DestinationRepository)
        {
            var exclitems = GetExclItems(ExclLogs);

            var processed = new HashSet<Guid>();

            var all = this.All();

            foreach (var item in all)
            {
                var changeitem = exclitems.Find(o => o.Id == item.Id);
                if (changeitem == null)
                {
                    DestinationRepository.AddOrUpdate(item);
                }
                else
                {
                    if (changeitem.Log.EditType == EditType.Update || changeitem.Log.EditType == EditType.Delete)
                    {
                        TValue value = this.Store.getValue(changeitem.Log.OldBlockPosition);
                        DestinationRepository.AddOrUpdate(value);
                    }
                    processed.Add(changeitem.Id);
                }
            }

            foreach (var item in exclitems.Where(o => !processed.Contains(o.Id)))
            {
                if (item.Log.EditType == EditType.Update || item.Log.EditType == EditType.Delete)
                {
                    TValue value = this.Store.getValue(item.Log.OldBlockPosition);
                    DestinationRepository.AddOrUpdate(value);
                }
            }
        }

        private List<ExclLogItem> GetExclItems(List<LogEntry> logs)
        {
            List<ExclLogItem> result = new List<ExclLogItem>();

            foreach (var item in logs.OrderBy(o => o.TimeTick))
            {
                var key = this.Store.KeyConverter.FromByte(item.KeyBytes);

                var exclitem = new ExclLogItem();
                exclitem.Id = key;
                exclitem.Log = item;
                result.Add(exclitem);
            }

            return result;

        }

        public void Reuild()
        {
            this.Store.OwnerDatabase.RebuildObjectStore<Guid, TValue>(this.Store, this.StoreParameters); 
        }

        public class ExclLogItem : IEquatable<ExclLogItem>
        {
            public Guid Id { get; set; }

            public LogEntry Log { get; set; }

            public override int GetHashCode()
            {
                return Lib.Security.Hash.ComputeInt(this.Id.ToString());
            }

            public bool Equals(ExclLogItem other)
            {
                return this.Id == other.Id;
            }
        }

        #endregion 

    }

    public interface ISiteRepositoryBase
    {
        SiteDb SiteDb { get; set; }
        void init();

        void Reuild(); 
    }
}
