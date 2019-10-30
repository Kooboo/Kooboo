//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Service
{
    public static class LogService
    {
        public static void RollBack(SiteDb siteDb, long logid)
        {
            var logentry = siteDb.Log.Store.get(logid);
            RollBack(siteDb, logentry);
        }

        public static void RollBack(SiteDb siteDb, LogEntry logentry)
        {
            if (logentry == null) { return; }

            if (logentry.IsTable)
            {
                var kdb = Kooboo.Data.DB.GetKDatabase(siteDb.WebSite);
                var table = kdb.GetOrCreateTable(logentry.TableName);
                table.RollBack(logentry);
            }
            else
            {
                var repo = siteDb.GetRepository(logentry.StoreName);
                repo?.RollBack(logentry);
            }
        }

        // when undelete a route, should try to undelete its destination object as well.
        public static void EnsureRestoreRouteObject(SiteDb siteDb, Route route)
        {
            // If this is an roll back of an route, if the destination item does not exists.. should try to roll back together..
            if (route.objectId != default(Guid))
            {
                var modeltype = Service.ConstTypeService.GetModelType(route.DestinationConstType);
                var repo = siteDb.GetRepository(modeltype);
                if (repo != null)
                {
                    var objectitme = repo.Get(route.objectId);
                    if (objectitme == null)
                    {
                        var lastitem = repo.GetLastEntryFromLog(route.objectId);
                        if (lastitem != null)
                        {
                            repo.AddOrUpdate(lastitem);
                        }
                    }
                }
            }
        }

        public static void EnsureRestoreObjectRoute(SiteDb siteDb, SiteObject siteobject)
        {
            if (siteobject != null && Attributes.AttributeHelper.IsRoutable(siteobject))
            {
                var route = siteDb.Routes.GetByObjectId(siteobject.Id);
                if (route == null)
                {
                    var repo = siteDb.Routes as IRepository;

                    var logs = siteDb.Log.GetByStoreName(typeof(Route).Name);
                    foreach (var item in logs.OrderByDescending(o => o.Id))
                    {
                        if (item.EditType == EditType.Delete)
                        {
                            var routeitem = repo.GetByLog(item);
                            if (routeitem is Route oldroute && oldroute.objectId == siteobject.Id)
                            {
                                siteDb.Routes.AddOrUpdate(oldroute);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void EnsureDeleteRouteObject(SiteDb siteDb, Route route)
        {
            if (route.objectId != default(Guid))
            {
                var modeltype = Service.ConstTypeService.GetModelType(route.DestinationConstType);
                var repo = siteDb.GetRepository(modeltype);
                var objectitem = repo?.Get(route.objectId);
                if (objectitem != null)
                {
                    repo.Delete(objectitem.Id);
                }
            }
        }

        public static void EnsureDeleteObjectRoute(SiteDb siteDb, SiteObject siteobject)
        {
            if (siteobject != null && Attributes.AttributeHelper.IsRoutable(siteobject))
            {
                var route = siteDb.Routes.GetByObjectId(siteobject.Id, siteobject.ConstType);
                if (route != null)
                {
                    siteDb.Routes.Delete(route.Id);
                }
            }
        }

        public static void RollBack(SiteDb siteDb, List<LogEntry> loglist)
        {
            var stores = loglist.Where(o => o.IsTable == false).ToList();

            foreach (var item in stores.GroupBy(o => o.StoreName))
            {
                var singleStoreList = item.ToList();

                var repo = siteDb.GetRepository(item.Key);

                repo?.RollBack(singleStoreList);
            }

            var tables = loglist.Where(o => o.IsTable).ToList();

            var kdb = Kooboo.Data.DB.GetKDatabase(siteDb.WebSite);

            foreach (var item in tables.GroupBy(o => o.TableName))
            {
                var tablelist = item.ToList();
                var table = kdb.GetOrCreateTable(item.Key);
                table.RollBack(tablelist);
            }
        }

        public static void RollBack(SiteDb siteDb, List<long> loglist)
        {
            List<LogEntry> logs = new List<LogEntry>();
            foreach (var item in loglist)
            {
                var log = siteDb.Log.Get(item);
                if (log != null)
                {
                    logs.Add(log);
                }
            }
            RollBack(siteDb, logs);
        }

        /// <summary>
        /// roll back the website back to certain version id.
        /// </summary>
        /// <param name="siteDb"></param>
        /// <param name="latestVersionId"></param>
        public static void RollBackFrom(SiteDb siteDb, long latestVersionId)
        {
            var logs = siteDb.Log.Store.Where(o => o.Id > latestVersionId).Take(99999);
            RollBack(siteDb, logs);
        }

        /// <summary>
        /// Check a new website, include the lastest log id.
        /// </summary>
        /// <param name="newDb"></param>
        /// <param name="latestLogId"></param>
        /// <param name="oldDb"></param>
        /// <param name="selfInclude"></param>
        public static void CheckOut(SiteDb oldDb, SiteDb newDb, long latestLogId, bool selfInclude = true)
        {
            foreach (var item in oldDb.ActiveRepositories())
            {
                var type = item.ModelType;
                if (Kooboo.Lib.Reflection.TypeHelper.HasInterface(item.ModelType, typeof(ICoreObject)))
                {
                    var destrepo = newDb.GetRepository(item.StoreName);
                    if (destrepo != null)
                    {
                        item.CheckOut(latestLogId, destrepo, true);
                    }
                }
            }

            foreach (var item in oldDb.TransferTasks.All())
            {
                newDb.TransferTasks.AddOrUpdate(item);
            }

            var kdb = Kooboo.Data.DB.GetKDatabase(oldDb.WebSite);
            var newkdb = Data.DB.GetKDatabase(newDb.WebSite);

            var alltables = kdb.GetTables();

            foreach (var item in alltables)
            {
                var currentable = kdb.GetTable(item);
                if (currentable != null)
                {
                    var table = newkdb.GetOrCreateTable(item);
                    currentable.CheckOut(latestLogId, table, selfInclude);
                }
            }

            var setting = Sync.ImportExport.GetSiteSetting(oldDb.WebSite);
            Sync.ImportExport.SetSiteSetting(newDb.WebSite, setting);
            Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(newDb.WebSite);
        }

        public static string GetStoreDisplayName(SiteDb sitedb, LogEntry log)
        {
            var repo = sitedb.GetRepository(log.StoreName);
            var siteobject = repo?.GetByLog(log);
            switch (siteobject)
            {
                case null:
                    return null;
                case ViewDataMethod viewDataMethod:
                {
                    var viewmethod = viewDataMethod;
                    var view = sitedb.Views.Get(viewmethod.ViewId);

                    return view == null ? viewmethod.AliasName : viewmethod.AliasName + "(" + view.Name + ")";
                }
                case IDataMethodSetting dataMethodSetting:
                {
                    var methodsetting = dataMethodSetting;
                    var type = Kooboo.Lib.Reflection.TypeHelper.GetType(methodsetting.DeclareType);
                    return type != null ? type.Name + "." + methodsetting.OriginalMethodName : methodsetting.OriginalMethodName;
                }
                default:
                {
                    var info = Service.ObjectService.GetObjectInfo(sitedb, siteobject);
                    return info?.DisplayName;
                }
            }
        }

        public static string GetLogDisplayName(SiteDb sitedb, LogEntry log, RenderContext context = null)
        {
            if (log.IsTable)
            {
                return GetTableDisplayName(sitedb, log, context);
            }
            else
            {
                return GetStoreDisplayName(sitedb, log);
            }
        }

        public static string GetTableDisplayName(SiteDb sitedb, LogEntry log, RenderContext context)
        {
            Dictionary<string, object> logdata = null;

            var table = Data.DB.GetTable(sitedb.DatabaseDb, log.TableName);

            if (table != null)
            {
                logdata = table.GetLogData(log);
            }
            return GetTableDisplayName(sitedb, log, context, logdata);
        }

        public static string GetTableDisplayName(SiteDb sitedb, LogEntry log, RenderContext context, Dictionary<string, object> logData)
        {
            string name = log.TableName;
            if (!string.IsNullOrWhiteSpace(log.TableColName))
            {
                name += ":" + log.TableColName;
            }

            if (logData != null)
            {
                logData.Remove("_id");

                string json = Lib.Helper.JsonHelper.Serialize(logData);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    return name + " " + Lib.Helper.StringHelper.GetSummary(json);
                }
            }

            return name;
        }

        public static Guid GetKeyHash(Guid key)
        {
            var bytes = Kooboo.IndexedDB.ByteConverter.GuidConverter.ConvertToByte(key);
            return Kooboo.Lib.Security.Hash.ComputeGuid(bytes);
        }
    }
}