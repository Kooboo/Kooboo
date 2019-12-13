//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Models;
using Kooboo.IndexedDB;
using Kooboo.Sites.Repository;
using Kooboo.Data.Interface;
using Kooboo.Sites.Routing;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Service
{
    public static class LogService
    {
        public static void RollBack(SiteDb SiteDb, long logid)
        {
            var logentry = SiteDb.Log.Store.get(logid);
            RollBack(SiteDb, logentry);

        }

        public static void RollBack(SiteDb SiteDb, LogEntry logentry)
        {
            if (logentry == null) { return; }

            if (logentry.IsTable)
            {
                var kdb = Kooboo.Data.DB.GetKDatabase(SiteDb.WebSite);
                var table = kdb.GetOrCreateTable(logentry.TableName);
                table.RollBack(logentry);
            }
            else
            {
                var repo = SiteDb.GetRepository(logentry.StoreName);
                if (repo != null)
                {
                    repo.RollBack(logentry);
                }
            }
        }

        // when undelete a route, should try to undelete its destination object as well. 
        public static void EnsureRestoreRouteObject(SiteDb SiteDb, Route Route)
        {
            /// If this is an roll back of an route, if the destination item does not exists.. should try to roll back together.. 
            if (Route.objectId != default(Guid))
            {
                var modeltype = Service.ConstTypeService.GetModelType(Route.DestinationConstType);
                var repo = SiteDb.GetRepository(modeltype);
                if (repo != null)
                {
                    var objectitme = repo.Get(Route.objectId);
                    if (objectitme == null)
                    {
                        var lastitem = repo.GetLastEntryFromLog(Route.objectId);
                        if (lastitem != null)
                        {
                            repo.AddOrUpdate(lastitem);
                        }
                    }
                }
            }

        }

        public static void EnsureRestoreObjectRoute(SiteDb SiteDb, SiteObject siteobject)
        {
            if (siteobject != null && Attributes.AttributeHelper.IsRoutable(siteobject))
            {
                var route = SiteDb.Routes.GetByObjectId(siteobject.Id);
                if (route == null)
                {
                    var repo = SiteDb.Routes as IRepository;

                    var logs = SiteDb.Log.GetByStoreName(typeof(Route).Name);
                    foreach (var item in logs.OrderByDescending(o => o.Id))
                    {
                        if (item.EditType == EditType.Delete)
                        {
                            var routeitem = repo.GetByLog(item);
                            if (routeitem != null)
                            {
                                var oldroute = routeitem as Route;
                                if (oldroute.objectId == siteobject.Id)
                                {
                                    SiteDb.Routes.AddOrUpdate(oldroute);
                                    return;
                                }
                            }
                        }
                    }

                }
            }
        }

        public static void EnsureDeleteRouteObject(SiteDb SiteDb, Route Route)
        {
            if (Route.objectId != default(Guid))
            {
                var modeltype = Service.ConstTypeService.GetModelType(Route.DestinationConstType);
                var repo = SiteDb.GetRepository(modeltype);
                if (repo != null)
                {
                    var Objectitem = repo.Get(Route.objectId);
                    if (Objectitem != null)
                    {
                        repo.Delete(Objectitem.Id);
                    }
                }
            }
        }

        public static void EnsureDeleteObjectRoute(SiteDb SiteDb, SiteObject siteobject)
        {
            if (siteobject != null && Attributes.AttributeHelper.IsRoutable(siteobject))
            {
                var route = SiteDb.Routes.GetByObjectId(siteobject.Id, siteobject.ConstType);
                if (route != null)
                {
                    SiteDb.Routes.Delete(route.Id);
                }
            }
        }


        public static void RollBack(SiteDb SiteDb, List<LogEntry> loglist)
        {
            var stores = loglist.Where(o => o.IsTable == false).ToList();

            foreach (var item in stores.GroupBy(o => o.StoreName))
            {
                var SingleStoreList = item.ToList();

                var repo = SiteDb.GetRepository(item.Key);

                if (repo != null)
                {
                    repo.RollBack(SingleStoreList);
                }
            }

            var tables = loglist.Where(o => o.IsTable).ToList();

            var kdb = Kooboo.Data.DB.GetKDatabase(SiteDb.WebSite);

            foreach (var item in tables.GroupBy(o => o.TableName))
            {
                var tablelist = item.ToList();
                var table = kdb.GetOrCreateTable(item.Key);
                table.RollBack(tablelist);
            }

        }

        public static void RollBack(SiteDb SiteDb, List<long> loglist)
        {
            List<LogEntry> logs = new List<LogEntry>();
            foreach (var item in loglist)
            {
                var log = SiteDb.Log.Get(item);
                if (log != null)
                {
                    logs.Add(log);
                }
            }
            RollBack(SiteDb, logs);
        }

        /// <summary>
        /// roll back the website back to certain version id. 
        /// </summary>
        /// <param name="website"></param>
        /// <param name="LatestVersionId"></param>
        public static void RollBackFrom(SiteDb SiteDb, long LatestVersionId)
        {
            var logs = SiteDb.Log.Store.Where(o => o.Id > LatestVersionId).Take(99999);
            RollBack(SiteDb, logs);
        }

        /// <summary>
        /// Check a new website, include the lastest log id. 
        /// </summary>
        /// <param name="OldWebSite"></param>
        /// <param name="NewWebSite"></param>
        /// <param name="LatestLogId"></param>
        public static void CheckOut(SiteDb OldDb, SiteDb NewDb, long LatestLogId, bool SelfInclude = true)
        {
            foreach (var item in OldDb.ActiveRepositories())
            {
                var type = item.ModelType;
                if (Kooboo.Lib.Reflection.TypeHelper.HasInterface(item.ModelType, typeof(ICoreObject)))
                {
                    var destrepo = NewDb.GetRepository(item.StoreName);
                    if (destrepo != null)
                    {
                        item.CheckOut(LatestLogId, destrepo, true);
                    }
                }
            }

            foreach (var item in OldDb.TransferTasks.All())
            {
                NewDb.TransferTasks.AddOrUpdate(item);
            }

            var kdb = Kooboo.Data.DB.GetKDatabase(OldDb.WebSite);
            var newkdb = Data.DB.GetKDatabase(NewDb.WebSite);

            var alltables = kdb.GetTables();

            foreach (var item in alltables)
            {
                var currentable = kdb.GetTable(item);
                if (currentable != null)
                {
                    var table = newkdb.GetTable(item);
                    if (table != null)
                    {
                        currentable.CheckOut(LatestLogId, table, SelfInclude, false);
                    } 
                }
            }

            var setting = Sync.ImportExport.GetSiteSetting(OldDb.WebSite);
            Sync.ImportExport.SetSiteSetting(NewDb.WebSite, setting);
            Kooboo.Data.GlobalDb.WebSites.AddOrUpdate(NewDb.WebSite);
        }

        public static string GetStoreDisplayName(SiteDb sitedb, LogEntry log)
        {
            var repo = sitedb.GetRepository(log.StoreName);
            var siteobject = repo?.GetByLog(log);
            if (siteobject == null)
            {
                return null;
            }
            if (siteobject is ViewDataMethod)
            {
                var viewmethod = siteobject as ViewDataMethod;
                var view = sitedb.Views.Get(viewmethod.ViewId);

                return view == null ? viewmethod.AliasName : viewmethod.AliasName + "(" + view.Name + ")";
            }
            if (siteobject is IDataMethodSetting)
            {
                var methodsetting = siteobject as IDataMethodSetting;
                var type = Kooboo.Lib.Reflection.TypeHelper.GetType(methodsetting.DeclareType);
                return type != null ? type.Name + "." + methodsetting.OriginalMethodName : methodsetting.OriginalMethodName;
            }
            var info = Service.ObjectService.GetObjectInfo(sitedb, siteobject as ISiteObject);
            return info != null ? info.DisplayName : null;
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

        public static string GetTableDisplayName(SiteDb sitedb, LogEntry log, RenderContext context, Dictionary<string, object> LogData)
        {
            string name = log.TableName;
            if (!string.IsNullOrWhiteSpace(log.TableColName))
            {
                name += ":" + log.TableColName;
            }

            if (LogData != null)
            {
                LogData.Remove("_id");

                string json = Lib.Helper.JsonHelper.Serialize(LogData);

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
