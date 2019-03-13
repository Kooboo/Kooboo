//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB;

namespace Kooboo.Sites.Repository
{
    public class ResourceGroupRepository : SiteRepositoryBase<ResourceGroup>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters param = new ObjectStoreParameters();
                param.AddColumn<ResourceGroup>(it => it.Name);
                param.AddColumn<ResourceGroup>(o => o.Id);
                param.AddColumn<ResourceGroup>(o => o.Type);
                param.AddColumn<ResourceGroup>(o => o.CreationDate);
                param.AddColumn<ResourceGroup>(o => o.LastModified);
                param.AddColumn("Name", 100);
                param.SetPrimaryKeyField<ResourceGroup>(o => o.Id);
                return param;
            }
        }

        public IEnumerable<ResourceGroup> GetStyleGroups()
        {
            return Query.Where(it => it.Type == ConstObjectType.Style).SelectAll();
        }

        public IEnumerable<ResourceGroup> GetScriptGroups()
        {
            return Query.Where(it => it.Type == ConstObjectType.Script).SelectAll();
        }

        public   bool IsUniqueName(string name, byte consttype)
        {
            Guid groupid = Kooboo.Data.IDGenerator.GetResourceGroupId(name, consttype);

            var existsItem = this.SiteDb.ResourceGroups.Get(groupid);
            if (existsItem != null)
            {
                return false;
            }

            string url = Sites.Service.GroupService.GetUrl(name, consttype);

            var route = this.SiteDb.Routes.GetByUrl(url);

            if (route == null)
            {
                return true; 
            }

            if (route.DestinationConstType != ConstObjectType.ResourceGroup)
            {
                return false;
            }

            if (route.objectId != default(Guid))
            {
                var group = this.SiteDb.ResourceGroups.Get(route.objectId); 
                if (group !=null)
                {
                    return false; 
                }
            }

            return true;
        }

        public ResourceGroup GetByNameOrId(string name, byte consttype = ConstObjectType.Style)
        {
            if (Lib.Helper.DataTypeHelper.IsGuid(name))
            {
                Guid gid = default(Guid);

                System.Guid.TryParse(name, out gid);

                return Get(gid);  
            }
            var id = Kooboo.Data.IDGenerator.GetResourceGroupId(name, consttype); 
            return Get(id);
        }
    }
}
