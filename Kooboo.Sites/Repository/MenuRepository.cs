//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class MenuRepository : SiteRepositoryBase<Menu>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddColumn<Menu>(o => o.ParentId);
                para.SetPrimaryKeyField<Menu>(o => o.Id);
                para.SetPrimaryKeyField<Menu>(o => o.Id);
                return para;
            }
        }

        public override Menu GetByNameOrId(string nameOrGuid)
        {
            bool parseok = Guid.TryParse(nameOrGuid, out var key);
            if (!parseok)
            {
                key = Data.IDGenerator.GetOrGenerate(nameOrGuid, ConstObjectType.Menu);
            }
            return Get(key);
        }

        public override bool AddOrUpdate(Menu value)
        {
            EnsureNewRender(value);
            return base.AddOrUpdate(value);
        }

        public override bool AddOrUpdate(Menu value, Guid userId)
        {
            EnsureNewRender(value);
            return base.AddOrUpdate(value, userId);
        }

        private void EnsureNewRender(Menu menu)
        {
            if (menu == null)
            {
                return;
            }
            menu.TempRenderData = null;

            if (menu.children != null)
            {
                foreach (var item in menu.children)
                {
                    EnsureNewRender(item);
                }
            }
        }

        private bool HasActive(Menu menu, ref string searchstring)
        {
            if (!string.IsNullOrEmpty(menu.Template) && menu.Template.Contains(searchstring))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(menu.SubItemTemplate) && menu.SubItemTemplate.Contains(searchstring))
            {
                return true;
            }

            foreach (var item in menu.children)
            {
                var result = HasActive(item, ref searchstring);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNameAvailable(string menuName)
        {
            var current = this.GetByNameOrId(menuName);
            return current == null;
        }

        public string GetNewMenuName()
        {
            string name = "";

            for (int i = 0; i < 1000; i++)
            {
                name = "untitled" + i.ToString();
                if (IsNameAvailable(name))
                {
                    return name;
                }
            }

            return null;
        }

        public void Swap(Guid rootId, Guid idA, Guid idB, Guid userId = default(Guid))
        {
            var menu = this.Get(rootId);
            if (menu != null)
            {
                if (_swap(menu, idA, idB))
                {
                    this.AddOrUpdate(menu, userId);
                }
            }
        }

        private bool _swap(Menu parentMenu, Guid idA, Guid idB)
        {
            if (parentMenu.children == null || parentMenu.children.Count == 0)
            {
                return false;
            }

            var founda = parentMenu.children.Find(o => o.Id == idA);
            var foundb = parentMenu.children.Find(o => o.Id == idB);

            if (founda != null && foundb != null)
            {
                int x = 0;
                int y = 0;

                for (int i = 0; i < parentMenu.children.Count; i++)
                {
                    var item = parentMenu.children[i];
                    if (item.Id == idA)
                    {
                        x = i;
                    }
                    if (item.Id == idB)
                    {
                        y = i;
                    }
                }

                if (x != y)
                {
                    var old = parentMenu.children[x];
                    parentMenu.children[x] = parentMenu.children[y];
                    parentMenu.children[y] = old;
                    return true;
                }
            }
            else if (founda == null && foundb == null)
            {
                foreach (var item in parentMenu.children)
                {
                    var ok = _swap(item, idA, idB);
                    if (ok)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}