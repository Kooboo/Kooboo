//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Repository
{
    public class RouteRepository : SiteRepositoryBase<Kooboo.Sites.Routing.Route>
    {
        private object _locker = new object();

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

        public void AddOrUpdate(string relativeUrl, byte constType, Guid objectId, Guid userId = default(Guid))
        {
            Route newroute = new Route {DestinationConstType = constType, objectId = objectId, Name = relativeUrl};
            AddOrUpdate(newroute, userId);
        }

        public void AddOrUpdate(string relativeUrl, Models.SiteObject siteobject, Guid userId = default(Guid))
        {
            Route newroute = new Route
            {
                DestinationConstType = siteobject.ConstType, objectId = siteobject.Id, Name = relativeUrl
            };
            AddOrUpdate(newroute, userId);
        }

        public override bool AddOrUpdate(Route value, Guid userId)
        {
            lock (_locker)
            {
                if (value.objectId != default(Guid))
                {
                    var route = this.GetByObjectId(value.objectId);
                    if (route != null && route.Name != value.Name)
                    {
                        this.Delete(route.Id, userId);
                    }
                }
                return base.AddOrUpdate(value, userId);
            }
        }

        public override bool AddOrUpdate(Route value)
        {
            return this.AddOrUpdate(value, default(Guid));
        }

        public void appendRoute(Route route, Guid userId)
        {
            base.AddOrUpdate(route, userId);
        }

        public void EnsureExists(string relativeUrl, byte constType)
        {
            var route = _getbyurl(relativeUrl);

            if (route == null)
            {
                lock (_locker)
                {
                    route = _getbyurl(relativeUrl);
                    if (route == null)
                    {
                        AddOrUpdate(relativeUrl, constType, default(Guid), default(Guid));
                    }
                }
            }
        }

        private Route _getbyurl(string url)
        {
            Route route = GetByUrl(url) ?? Kooboo.Sites.Routing.ObjectRoute.GetRoute(this.SiteDb, url);
            return route;
        }

        public List<Route> GetByType(byte constType)
        {
            return this.Query.Where(o => o.DestinationConstType == constType).SelectAll();
        }

        /// <summary>
        /// rename.....
        /// </summary>
        /// <param name="oldRelativeUrl"></param>
        /// <param name="newRelativeUrl"></param>
        public void ChangeRoute(string oldRelativeUrl, string newRelativeUrl)
        {
            //TODO: this is a rename, should change
            var oldroute = GetByUrl(oldRelativeUrl);

            if (oldroute != null)
            {
                Route newroute = new Route
                {
                    DestinationConstType = oldroute.DestinationConstType,
                    objectId = oldroute.objectId,
                    Name = newRelativeUrl
                };

                AddOrUpdate(newroute);
                Delete(oldroute.Id);

                Sync.DiskSyncHelper.ChangeRoute(this.SiteDb, oldRelativeUrl, newroute.Name);
            }
        }

        public override Route GetByUrl(string relativeUrl)
        {
            return Get(Data.IDGenerator.GetRouteId(relativeUrl));
        }

        public Route GetByObjectId(Guid objectId, byte destinationConstType = 0)
        {
            if (destinationConstType == 0)
            {
                return Store.Where(o => o.objectId == objectId).FirstOrDefault();
            }
            else
            {
                return Store.Where(o => o.objectId == objectId && o.DestinationConstType == destinationConstType).FirstOrDefault();
            }
        }

        public void Delete(string relativeUrl)
        {
            Route route = GetByUrl(relativeUrl);
            if (route != null)
            {
                Delete(route.Id);
            }
        }

        public override void Delete(Guid id, Guid userId)
        {
            lock (_locker)
            {
                base.Delete(id, userId);
                DeleteAlias(id, userId);
            }
        }

        public override void Delete(Guid id)
        {
            this.Delete(id, default(Guid));
        }

        internal void DeleteAlias(Guid routeid, Guid userid)
        {
            var list = Store.Where(o => o.objectId == routeid && o.DestinationConstType == ConstObjectType.Route).SelectAll();
            foreach (var item in list)
            {
                this.Delete(item.Id, userid);
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
            var route = this.Store.Where(o => o.objectId == objectId).FirstOrDefault();

            if (route != null && !string.IsNullOrEmpty(route.Name))
            {
                return route.Name;
            }
            return string.Empty;
        }

        public bool Validate(string routeName, Guid objectId)
        {
            if (!routeName.StartsWith("/"))
            {
                routeName = "/" + routeName;
            }

            var route = this.GetByUrl(routeName);
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
                    if (route.objectId == objectId)
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