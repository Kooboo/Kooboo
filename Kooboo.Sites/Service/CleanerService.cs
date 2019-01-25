//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
    public static class CleanerService
    {
        public static void CleanDataMethod(SiteDb sitedb)
        {
            var allmethods = sitedb.DataMethodSettings.Query.Where(o => o.IsPublic == false).SelectAll();
            var allviewmethods = sitedb.ViewDataMethods.All();
            var allmethodids = GetAllMethods(allviewmethods);

            foreach (var item in allmethods)
            {
                if (!allmethodids.Contains(item.Id))
                {
                    sitedb.DataMethodSettings.Delete(item.Id);
                }
            }
        }


        public static List<Guid> GetAllMethods(List<ViewDataMethod> viewmethods)
        {
            List<Guid> result = new List<Guid>();
            foreach (var item in viewmethods)
            {
                AddToList(item, ref result);
            }
            return result;
        }

        private static void AddToList(ViewDataMethod method, ref List<Guid> result)
        {
            if (method.MethodId != default(Guid))
            {
                result.Add(method.MethodId);
                if (method.HasChildren)
                {
                    foreach (var item in method.Children)
                    {
                        AddToList(item, ref result);
                    }
                }
            }
        }
    }
}
