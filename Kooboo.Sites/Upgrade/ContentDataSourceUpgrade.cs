//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Upgrade
{
    public class ContentDataSourceUpgrade : IApplicationUpgrade
    {
        public System.Version LowerVersion
        {
            get
            {
                return new Version("0.0.0.0");
            }
        }

        public System.Version UpVersion
        {
            get
            {
                return new Version("0.2.0.0");
            }
        }

        // migrate all contentdatasource.
        public void Do()
        {
            var allsites = Kooboo.Data.GlobalDb.WebSites.All();
            foreach (var item in allsites)
            {
                var sitedb = item.SiteDb();

                var methods = sitedb.DataMethodSettings.All();

                foreach (var method in methods)
                {
                    if (method.DeclareType.Contains("ContentDataSource"))
                    {
                        var newMethod = UpdateToNewMethod(sitedb, method);

                        if (newMethod != null)
                        {
                            sitedb.DataMethodSettings.AddOrUpdate(newMethod);
                            var viewmethods = sitedb.ViewDataMethods.Query.Where(o => o.MethodId == method.Id).SelectAll();
                            foreach (var viewmethod in viewmethods)
                            {
                                viewmethod.MethodId = newMethod.Id;
                                sitedb.ViewDataMethods.AddOrUpdate(viewmethod);
                            }
                        }
                    }
                }
            }
        }

        public DataMethodSetting UpdateToNewMethod(SiteDb sitedb, DataMethodSetting oldmethod)
        {
            DataMethodSetting newMethod = null;
            if (oldmethod.OriginalMethodName == "GetById")
            {
                newMethod = ConvertGetById(oldmethod);
            }
            else if (oldmethod.OriginalMethodName == "GetByUserKey")
            {
                newMethod = ConvertGetByUserKey(oldmethod);
            }
            else if (oldmethod.OriginalMethodName == "List")
            {
                newMethod = ConvertList(oldmethod);
            }
            else if (oldmethod.OriginalMethodName == "ListByCategoryId")
            {
                newMethod = ConvertListByCategoryId(oldmethod);
            }
            else if (oldmethod.OriginalMethodName == "ListByCategoryKey")
            {
                newMethod = ConvertListByCategoryKey(oldmethod);
            }

            return newMethod;
        }

        public DataMethodSetting ConvertGetById(DataMethodSetting oldmethod)
        {
            var newmethod = Data.GlobalDb.DataMethodSettings.TableScan.Where(o => o.MethodName == "ById" && o.DeclareType.Contains("ContentItem")).FirstOrDefault();

            var newseting = CreateNewSetting(oldmethod, newmethod);
            var binding = getbinding(oldmethod.ParameterBinding, "id");
            var newbinding = getbinding(newseting.ParameterBinding, "Id");
            newbinding.Binding = binding.Binding;
            return newseting;
        }

        private DataMethodSetting CreateNewSetting(DataMethodSetting oldmethod, DataMethodSetting newmethod)
        {
            DataMethodSetting newMethodSetting = new DataMethodSetting
            {
                MethodName = oldmethod.MethodName,
                Parameters = newmethod.Parameters,
                ParameterBinding = newmethod.ParameterBinding,
                Description = oldmethod.Description,
                MethodSignatureHash = newmethod.MethodSignatureHash,
                OriginalMethodName = newmethod.OriginalMethodName,
                DeclareType = newmethod.DeclareType,
                DeclareTypeHash = newmethod.DeclareTypeHash,
                IsPublic = oldmethod.IsPublic,
                ReturnType = newmethod.ReturnType,
                IsPagedResult = newmethod.IsPagedResult,
                IsPost = newmethod.IsPost,
                IsStatic = newmethod.IsStatic,
                IsTask = newmethod.IsTask,
                IsVoid = newmethod.IsVoid
            };

            return newMethodSetting;
        }

        public DataMethodSetting ConvertGetByUserKey(DataMethodSetting oldmethod)
        {
            var newmethod = Data.GlobalDb.DataMethodSettings.TableScan.Where(o => o.MethodName == "ByUserKey" && o.DeclareType.Contains("ContentItem")).FirstOrDefault();

            var newseting = CreateNewSetting(oldmethod, newmethod);

            var binding = getbinding(oldmethod.ParameterBinding, "UserKey");

            var newbinding = getbinding(newseting.ParameterBinding, "UserKey");

            newbinding.Binding = binding.Binding;

            return newseting;
        }

        public DataMethodSetting ConvertList(DataMethodSetting oldmethod)
        {
            var newmethod = Data.GlobalDb.DataMethodSettings.TableScan.Where(o => o.MethodName == "ByFolder" && o.DeclareType.Contains("ContentList")).FirstOrDefault();

            return ConvertContentListPara(oldmethod, newmethod);
        }

        private DataMethodSetting ConvertContentListPara(DataMethodSetting oldmethod, DataMethodSetting newmethod)
        {
            var newMethodSetting = CreateNewSetting(oldmethod, newmethod);

            var bindingFolderid = getbinding(oldmethod.ParameterBinding, "FolderId");
            var newbindingfolderid = getbinding(newMethodSetting.ParameterBinding, "FolderId");
            if (bindingFolderid != null && Lib.Helper.DataTypeHelper.IsGuid(bindingFolderid.Binding))
            {
                newbindingfolderid.Binding = bindingFolderid.Binding;
            }

            var filter = getbinding(oldmethod.ParameterBinding, "Filter");
            var newFilter = getbinding(newMethodSetting.ParameterBinding, "Filters");

            if (filter != null && !string.IsNullOrEmpty(filter.Binding) && Lib.Helper.DataTypeHelper.IsJsonType(filter.Binding, typeof(List<FilterDefinition>)))
            {
                newFilter.Binding = filter.Binding;
            }

            var pagesize = getbinding(oldmethod.ParameterBinding, "PageSize");
            var newPageSize = getbinding(newMethodSetting.ParameterBinding, "PageSize");

            if (pagesize != null && !string.IsNullOrEmpty(pagesize.Binding) && Lib.Helper.DataTypeHelper.IsInt(pagesize.Binding))
            {
                newPageSize.Binding = pagesize.Binding;
            }

            var pagenumber = getbinding(oldmethod.ParameterBinding, "PageNumber");
            var newPageNumber = getbinding(newMethodSetting.ParameterBinding, "PageNumber");

            if (pagenumber != null && !string.IsNullOrEmpty(pagenumber.Binding) && Lib.Helper.DataTypeHelper.IsInt(pagenumber.Binding))
            {
                newPageNumber.Binding = pagenumber.Binding;
            }

            var sortField = getbinding(oldmethod.ParameterBinding, "SortField");
            var newSortField = getbinding(newMethodSetting.ParameterBinding, "SortField");

            if (sortField != null && !string.IsNullOrEmpty(sortField.Binding) && !sortField.Binding.Contains("{"))
            {
                newSortField.Binding = sortField.Binding;
            }

            var ascending = getbinding(oldmethod.ParameterBinding, "IsAscending");
            var newAscending = getbinding(newMethodSetting.ParameterBinding, "IsAscending");

            if (ascending != null && !string.IsNullOrEmpty(ascending.Binding) && Lib.Helper.DataTypeHelper.IsBool(ascending.Binding))
            {
                newAscending.Binding = ascending.Binding;
            }

            return newMethodSetting;
        }

        public DataMethodSetting ConvertListByCategoryId(DataMethodSetting oldmethod)
        {
            var newmethod = Data.GlobalDb.DataMethodSettings.TableScan.Where(o => o.MethodName == "ByCategoryId" && o.DeclareType.Contains("ContentList")).FirstOrDefault();

            var newsetting = ConvertContentListPara(oldmethod, newmethod);

            var id = getbinding(oldmethod.ParameterBinding, "Id");
            var newId = getbinding(newsetting.ParameterBinding, "Id");

            if (id != null && !string.IsNullOrEmpty(id.Binding))
            {
                newId.Binding = id.Binding;
            }

            return newsetting;
        }

        public DataMethodSetting ConvertListByCategoryKey(DataMethodSetting oldmethod)
        {
            var newmethod = Data.GlobalDb.DataMethodSettings.TableScan.Where(o => o.MethodName == "ByCategoryKey" && o.DeclareType.Contains("ContentList")).FirstOrDefault();

            var newsetting = ConvertContentListPara(oldmethod, newmethod);

            var key = getbinding(oldmethod.ParameterBinding, "Userkey");
            var newkey = getbinding(newsetting.ParameterBinding, "UserKey");

            if (key != null && !string.IsNullOrEmpty(key.Binding))
            {
                newkey.Binding = key.Binding;
            }
            return newsetting;
        }

        private ParameterBinding getbinding(Dictionary<string, ParameterBinding> parameterBinding, string name)
        {
            name = name.ToLower();
            if (parameterBinding == null)
            {
                return null;
            }
            foreach (var item in parameterBinding)
            {
                if (item.Key.ToLower() == name)
                {
                    return item.Value;
                }
            }

            foreach (var item in parameterBinding)
            {
                if (item.Key.ToLower().Contains(name))
                {
                    return item.Value;
                }
            }

            return null;
        }
    }
}