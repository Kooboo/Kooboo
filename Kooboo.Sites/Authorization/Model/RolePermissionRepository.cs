using Kooboo.IndexedDB;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization.Model
{

    public class RolePermissionRepository : SiteRepositoryBase<RolePermission>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.SetPrimaryKeyField<RolePermission>(o => o.Id);
                return para;
            }
        }

        public override RolePermission Get(Guid id, bool getColumnDataOnly = false)
        {
            var item = base.Get(id, getColumnDataOnly);
            if (item != null)
            {
                return item;
            }
            else
            {
                return DefaultData.GetDefault(id);
            }
        }

        public override RolePermission Get(string nameorid)
        {
            return GetByNameOrId(nameorid);
        }

        public override RolePermission GetByNameOrId(string NameOrGuid)
        {

            Guid key;
            bool parseok = Guid.TryParse(NameOrGuid, out key);

            if (!parseok)
            {
                byte consttype = ConstTypeContainer.GetConstType(typeof(RolePermission));

                key = Data.IDGenerator.Generate(NameOrGuid, consttype);
            }
            return Get(key, false);
        }

        public override List<RolePermission> All()
        {
            var list = base.All();
            AppendDefault(list);
            return list;
        }


        public void AppendDefault(List<RolePermission> current)
        {
            if (current.Find(o => o.Id == DefaultData.Master.Id) == null)
            {
                current.Add(DefaultData.Master);
            }

            if (current.Find(o => o.Id == DefaultData.Developer.Id) == null)
            {
                current.Add(DefaultData.Developer);
            }

            if (current.Find(o => o.Id == DefaultData.ContentManager.Id) == null)
            {
                current.Add(DefaultData.ContentManager);
            }

        }

        public override RolePermission GetFromCache(Guid id)
        {
            var item = base.GetFromCache(id);
            if (item != null)
            {
                return item;
            }
            return DefaultData.GetDefault(id);
        }


        public override List<RolePermission> List(bool UseColumnData = false)
        {
            var list = base.List(UseColumnData);
            list.Add(DefaultData.Master);
            list.Add(DefaultData.Developer);
            list.Add(DefaultData.ContentManager);
            return list;
        }

        public override bool AddOrUpdate(RolePermission value, Guid UserId)
        {
            if (string.IsNullOrWhiteSpace(value.Name))
            {
                return false;
            }


            //  return base.AddOrUpdate(value, UserId);

            lock (_locker)
            {
                var old = this.Store.get(value.Id);
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
         


        public override bool AddOrUpdate(RolePermission value)
        {
            return AddOrUpdate(value, default(Guid));
        }



        public override void Delete(Guid id)
        {
            this.Delete(id, default(Guid));
        }

        public override void Delete(Guid id, Guid UserId)
        {
            if (id==DefaultData.Master.Id || id == DefaultData.Developer.Id || id == DefaultData.ContentManager.Id)
            {
                throw new Exception("default role can not be deleted"); 
            }
            var old = this.Store.get(id);
            if (old != null)
            {
                RaiseBeforeEvent(old, ChangeType.Delete);
                this.Store.CurrentUserId = UserId;
                Store.delete(id);
            }
            RaiseEvent(old, ChangeType.Delete);
        }

    }





}




//public static class Actions
//{

//    public static uint Admin = Hierarchy.GetInt(1);

//    public static uint System = Hierarchy.GetInt(1, 1);

//    public static class Systems
//    {
//        public static uint Settings = Hierarchy.GetInt(1, 1, 1);

//        public static uint Domains = Hierarchy.GetInt(1, 1, 2);

//        public static uint SiteLogs = Hierarchy.GetInt(1, 1, 3);

//        public static uint VisitorLogs = Hierarchy.GetInt(1, 1, 4);

//        public static uint Disk = Hierarchy.GetInt(1, 1, 5);

//        public static uint Jobs = Hierarchy.GetInt(1, 1, 6);

//        public static uint SiteUser = Hierarchy.GetInt(1, 1, 7);

//        public static uint Events = Hierarchy.GetInt(1, 1, 8);

//        public static uint Synchronization = Hierarchy.GetInt(1, 1, 9);

//        public static uint Configs = Hierarchy.GetInt(1, 1, 10);
//    }

//    public static uint Development = Hierarchy.GetInt(1, 2);

//    public static class Developments
//    {
//        public static uint Layouts = Hierarchy.GetInt(1, 2, 1);
//        public static uint Views = Hierarchy.GetInt(1, 2, 1);
//        public static uint Forms = Hierarchy.GetInt(1, 2, 1);
//        public static uint Menus = Hierarchy.GetInt(1, 2, 1);
//        public static uint Scripts = Hierarchy.GetInt(1, 2, 1);

//        public static uint Styles = Hierarchy.GetInt(1, 2, 1);
//        public static uint Code = Hierarchy.GetInt(1, 2, 1);
//        public static uint Urls = Hierarchy.GetInt(1, 2, 1);
//        public static uint Search = Hierarchy.GetInt(1, 2, 1);

//        public static uint DataSource = Hierarchy.GetInt(1, 2, 1);

//    }

//    public static uint Content = Hierarchy.GetInt(1, 3);

//    public static class Contents
//    {
//        public static uint Content = Hierarchy.GetInt(1, 3, 1);
//        public static uint Labels = Hierarchy.GetInt(1, 3, 2);
//        public static uint HtmlBlocks = Hierarchy.GetInt(1, 3, 3);
//        public static uint ContentTypes = Hierarchy.GetInt(1, 3, 4);

//        public static uint Multilingual = Hierarchy.GetInt(1, 3, 5);

//    }

//    public static uint Storage = Hierarchy.GetInt(1, 4);

//    public static class Storages
//    {
//        public static uint Database = Hierarchy.GetInt(1, 4, 1);
//        public static uint KeyValue = Hierarchy.GetInt(1, 4, 2);
//        public static uint Files = Hierarchy.GetInt(1, 4, 3);
//    }
//}