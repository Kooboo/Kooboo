//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data;
using Kooboo.IndexedDB;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class RouteRepository : SiteRepositoryBase<Kooboo.Sites.Routing.Route>
    {

        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.AddIndex<Route>(o => o.objectId);
                paras.AddColumn<Route>(o => o.DestinationConstType);
                paras.SetPrimaryKeyField<Route>(o => o.Id);
                return paras;
            }
        }

        public void AddOrUpdate(string relativeUrl, byte ConstType, Guid objectId, Guid UserId = default(Guid))
        {
            Route newroute = new Route();
            newroute.DestinationConstType = ConstType;
            newroute.objectId = objectId;
            newroute.Name = relativeUrl;
            AddOrUpdate(newroute, UserId);
        }

        public void AddOrUpdate(string relativeUrl, Models.SiteObject siteobject, Guid UserId = default(Guid))
        {
            Route newroute = new Route();
            newroute.DestinationConstType = siteobject.ConstType;
            newroute.objectId = siteobject.Id;
            newroute.Name = relativeUrl;
            AddOrUpdate(newroute, UserId);
        }

        public override bool AddOrUpdate(Route value, Guid UserId)
        {
            lock (_locker)
            {
                if (value.objectId != default(Guid))
                {
                    var route = this.GetByObjectId(value.objectId);
                    if (route != null && route.Name != value.Name)
                    {
                        this.Delete(route.Id, UserId);
                    }
                }
                return base.AddOrUpdate(value, UserId);
            }
        }

        public override bool AddOrUpdate(Route value)
        {
            return this.AddOrUpdate(value, default(Guid));
        }

        public void appendRoute(Route route, Guid UserId)
        {
            base.AddOrUpdate(route, UserId);
        }

        public void EnsureExists(string relativeUrl, byte ConstType)
        {
            var route = _getbyurl(relativeUrl);

            if (route == null)
            {
                lock (_locker)
                {
                    route = _getbyurl(relativeUrl);
                    if (route == null)
                    {
                        AddOrUpdate(relativeUrl, ConstType, default(Guid), default(Guid));
                    }
                }
            }
        }

        private Route _getbyurl(string url)
        {
            Route route = GetByUrl(url);
            if (route == null)
            {
                route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(this.SiteDb, url);
            }
            return route;
        }


        public List<Route> GetByType(byte ConstType)
        {
            return this.Query.Where(o => o.DestinationConstType == ConstType).SelectAll();
        }

        /// <summary>
        /// rename..... 
        /// </summary>
        /// <param name="OldRelativeUrl"></param>
        /// <param name="NewRelativeUrl"></param>
        public void ChangeRoute(string OldRelativeUrl, string NewRelativeUrl)
        {
            ///TODO: this is a rename, should change 
            var oldroute = GetByUrl(OldRelativeUrl);

            if (oldroute != null)
            {
                Route newroute = new Route();
                newroute.DestinationConstType = oldroute.DestinationConstType;
                newroute.objectId = oldroute.objectId;
                newroute.Name = NewRelativeUrl;

                AddOrUpdate(newroute);
                Delete(oldroute.Id);

                Sync.DiskSyncHelper.ChangeRoute(this.SiteDb, OldRelativeUrl, newroute.Name);

            }

        }

        public override Route GetByUrl(string relativeUrl)
        {
            return Get(Data.IDGenerator.GetRouteId(relativeUrl));
        }

        public Route GetByObjectId(Guid objectId, byte DestinationConstType = 0)
        {
            if (DestinationConstType == 0)
            {
                return Store.Where(o => o.objectId == objectId).FirstOrDefault();
            }
            else
            {
                return Store.Where(o => o.objectId == objectId && o.DestinationConstType == DestinationConstType).FirstOrDefault();
            }
        }

        public void Delete(string RelativeUrl)
        {
            Route route = GetByUrl(RelativeUrl);
            if (route != null)
            {
                Delete(route.Id);
            }
        }

        public override void Delete(Guid id, Guid UserId)
        {
            lock (_locker)
            {
                base.Delete(id, UserId);
                DeleteAlias(id, UserId);
            }

        }

        public override void Delete(Guid id)
        {
            this.Delete(id, default(Guid));
        }

        internal void DeleteAlias(Guid routeid, Guid Userid)
        {
            var list = Store.Where(o => o.objectId == routeid && o.DestinationConstType == ConstObjectType.Route).SelectAll();
            foreach (var item in list)
            {
                this.Delete(item.Id, Userid);
            }
            // delete resource group.. 
            var relations = this.SiteDb.Relations.GetReferredBy(typeof(Route), routeid, ConstObjectType.ResourceGroup);
            if (relations != null && relations.Count > 0)
            {
                foreach (var item in relations)
                {
                    var group = this.SiteDb.ResourceGroups.Get(item.objectXId);
                    if (group != null && group.Children != null && group.Children.ContainsKey(routeid))
                    {
                        group.Children.Remove(routeid);
                        SiteDb.ResourceGroups.AddOrUpdate(group);
                    }
                }
            }
        }

        public string GetObjectPrimaryRelativeUrl(Guid objectId)
        {
            if (objectId == default(Guid))
            {
                return string.Empty;
            }

            var route = this.Store.Where(o => o.objectId == objectId).FirstOrDefault();

            if (route != null && !string.IsNullOrEmpty(route.Name))
            {
                return route.Name;
            }
            return string.Empty;
        }


        public bool Validate(string RouteName, Guid ObjectId)
        {
            if (!RouteName.StartsWith("/"))
            {
                RouteName = "/" + RouteName;
            }

            var route = this.GetByUrl(RouteName);
            if (route == null)
            {
                return true;
            }
            else
            {
                if (route.objectId == default(Guid))
                {
                    return true;
                }
                else
                {
                    if (route.objectId == ObjectId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }
    }
}
