//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.DataSources.New.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Web.Areas.Admin.ViewModels;

namespace Kooboo.Web.Api.Implementation
{
    public class DataMethodSettingApi : SiteObjectApi<DataMethodSetting>
    {
        public List<DataSourceViewModel> ListDataSource(ApiCall call)
        {
            return FilterOut(allmethods(call), call.WebSite);
        }

        public List<DataSourceViewModel> CenterList(ApiCall call)
        {
            return FilterOut(allmethods(call), call.WebSite);
        }

        public List<DataSourceViewModel> Public(ApiCall call)
        {
            var all = FilterOut(allmethods(call), call.WebSite);
            foreach (var item in all)
            {
                item.Methods.RemoveAll(o => !o.IsGlobal && !o.IsPublic);
            }
            SetRelation(all, call.WebSite.SiteDb());
            return all;
        }

        public List<DataSourceViewModel> Private(ApiCall apiCall)
        {
            var all = FilterOut(allmethods(apiCall), apiCall.WebSite);
            foreach (var item in all)
            {
                item.Methods.RemoveAll(o => o.IsGlobal || o.IsPublic);
            }
            SetRelation(all, apiCall.WebSite.SiteDb());
            return all;
        }

        private List<DataSourceViewModel> allmethods(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var globalSettings = GlobalDb.DataMethodSettings.All();
            var siteSettings = sitedb.DataMethodSettings.All();
            List<DataSourceViewModel> viewModels = new List<DataSourceViewModel>();

            AppendMethod(sitedb, globalSettings, true, viewModels);
            AppendMethod(sitedb, siteSettings, false, viewModels);

            AppendCode(sitedb, viewModels);

            return viewModels.OrderBy(o => o.Type).ToList();
        }

        public List<DataSourceViewModel> ByView(ApiCall call)
        {
            Guid viewid = default(Guid);
            if (call.ObjectId != default(Guid))
            {
                viewid = call.ObjectId;
            }
            else
            {
                string value = call.GetValue("viewid", "view");

                if (!string.IsNullOrEmpty(value))
                {
                    System.Guid.TryParse(value, out viewid);
                }
            }

            var all = FilterOut(allmethods(call), call.WebSite);

            List<Guid> privateMethodIds = new List<Guid>();
            if (viewid != default(Guid))
            {
                var allviewmethods = call.WebSite.SiteDb().ViewDataMethods.FlatListByView(viewid);
                privateMethodIds = allviewmethods.Select(o => o.MethodId).ToList();
            }
            List<DataSourceViewModel> SourceRemove = new List<DataSourceViewModel>();

            foreach (var item in all)
            {
                var toberemoved = item.Methods.Where(o => o.IsPublic == false && o.IsGlobal == false && !privateMethodIds.Contains(o.Id)).ToList();

                foreach (var method in toberemoved)
                {
                    item.Methods.Remove(method);
                }

                if (item.Methods.Count() == 0)
                {
                    SourceRemove.Add(item);
                }
            }

            foreach (var item in SourceRemove)
            {
                all.Remove(item);
            }

            return all;
        }

        private void AppendMethod(SiteDb siteDb, List<DataMethodSetting> methods, bool isGlobal, List<DataSourceViewModel> target)
        {
            var groups = methods.GroupBy(o => o.DeclareType);
            foreach (var item in groups)
            {
                var dataSource = target.FirstOrDefault(o => o.Type == item.Key);
                if (dataSource == null)
                {
                    var first = item.FirstOrDefault();
                    dataSource = new DataSourceViewModel()
                    {
                        Type = item.Key
                    };
                    if (first != null)
                    {
                        dataSource.IsThridPartyDataSource = first.IsThirdPartyType;
                        dataSource.TypeHash = first.DeclareTypeHash;
                    }
                    target.Add(dataSource);
                }

                // TODO... check that kind of datamethod that is from code. 

                foreach (var dataMethod in item)
                {
                    dataSource.AddMethod(siteDb, dataMethod, isGlobal);
                }
            }
        }

        private void AppendCode(SiteDb sitedb, List<DataSourceViewModel> target)
        {
            // append the global code...NOT the one in the site datemethod setting. 
            // but those to be set... 
            var settings = Kooboo.Sites.DataSources.ScriptSourceManager.GetCodeMethods(sitedb);

            List<DataMethodSetting> newsetting = new List<DataMethodSetting>();

            foreach (var item in settings)
            {
                var righttype = target.Find(o => o.Type == item.DeclareType);

                if (righttype == null || !righttype.Methods.Any(o => o.Id == item.Id))
                {
                    newsetting.Add(item);
                }
            }

            AppendMethod(sitedb, newsetting, false, target);
        }

        [Kooboo.Attributes.RequireModel(typeof(DataMethodViewModel))]
        public DataMethodViewModel Add(ApiCall call)
        {
            DataMethodViewModel viewmodel = call.Context.Request.Model as DataMethodViewModel;

            if (viewmodel.IsNew)
            {
                var methodhash = viewmodel.MethodSignatureHash;
                IDataMethodSetting originalSetting = GlobalDb.DataMethodSettings.Query.Where(o => o.MethodSignatureHash == methodhash).FirstOrDefault();
                DataMethodSetting newMethodSetting = new DataMethodSetting
                {
                    MethodName = viewmodel.MethodName,
                    ParameterBinding = viewmodel.ParameterBinding,
                    Parameters = viewmodel.Parameters,
                    Description = viewmodel.Description,
                    OriginalMethodName = originalSetting.OriginalMethodName,
                    MethodSignatureHash = originalSetting.MethodSignatureHash,
                    DeclareType = originalSetting.DeclareType,
                    ReturnType = originalSetting.ReturnType,
                    DeclareTypeHash = originalSetting.DeclareTypeHash,
                };
                if (!viewmodel.IsPublic)
                {
                    newMethodSetting.MethodName = System.Guid.NewGuid().ToString();
                }
                newMethodSetting.IsPublic = viewmodel.IsPublic;
                call.WebSite.SiteDb().DataMethodSettings.AddOrUpdate(newMethodSetting, call.Context.User.Id);
                viewmodel = UpdateViewModel(viewmodel, call.WebSite.SiteDb(), newMethodSetting);
            }
            else
            {
                var dataMethodSetting = call.WebSite.SiteDb().DataMethodSettings.Get(viewmodel.Id);
                dataMethodSetting.ParameterBinding = viewmodel.ParameterBinding;
                dataMethodSetting.Description = viewmodel.Description;

                call.WebSite.SiteDb().DataMethodSettings.AddOrUpdate(dataMethodSetting, call.Context.User.Id);
                viewmodel = UpdateViewModel(viewmodel, call.WebSite.SiteDb(), dataMethodSetting);
            }

            return viewmodel;
        }


        private void CheckCorrectSampleJson(DataMethodViewModel model, ApiCall call)
        {
            if (model != null && model.ParameterBinding != null)
            {
                foreach (var item in model.ParameterBinding)
                {
                    if (item.Key.ToLower() == Kooboo.Sites.DataSources.ScriptSourceManager.SampleResponseFieldName.ToLower())
                    {
                        var value = item.Value.Binding;
                        if (value != null)
                        {
                            var obj = Lib.Helper.JsonHelper.Deserialize(value);
                        }
                    }
                }
            }
        }


        public TypeInfoModel Update(ApiCall call)
        {
            string json = call.Context.Request.Body;

            var sitedb = call.Context.WebSite.SiteDb();

            DataMethodViewModel viewmodel = Lib.Helper.JsonHelper.Deserialize<DataMethodViewModel>(json);

            CheckCorrectSampleJson(viewmodel, call);

            if (viewmodel.Id == default(Guid))
            {
                // TODO: check if it is from the kscript source as well.. 
                var methodhash = viewmodel.MethodSignatureHash;

                IDataMethodSetting originalSetting;

                originalSetting = GlobalDb.DataMethodSettings.Query.Where(o => o.MethodSignatureHash == methodhash).FirstOrDefault();

                if (originalSetting == null)
                {
                    originalSetting = Kooboo.Sites.DataSources.ScriptSourceManager.GetByMethodHash(sitedb, methodhash);
                }

                if (originalSetting != null)
                {
                    DataMethodSetting newMethodSetting = new DataMethodSetting
                    {
                        MethodName = viewmodel.MethodName,
                        ParameterBinding = viewmodel.ParameterBinding,
                        Parameters = viewmodel.Parameters,
                        Description = viewmodel.Description,
                        OriginalMethodName = originalSetting.OriginalMethodName,
                        MethodSignatureHash = originalSetting.MethodSignatureHash,
                        DeclareType = originalSetting.DeclareType,
                        ReturnType = originalSetting.ReturnType,
                        DeclareTypeHash = originalSetting.DeclareTypeHash,
                        IsPagedResult = originalSetting.IsPagedResult
                    };

                    if (!viewmodel.IsPublic)
                    {
                        newMethodSetting.MethodName = System.Guid.NewGuid().ToString();
                    }
                    newMethodSetting.IsPublic = viewmodel.IsPublic;

                    call.WebSite.SiteDb().DataMethodSettings.AddOrUpdate(newMethodSetting, call.Context.User.Id);

                    var fields = DataSourceHelper.GetFields(call.WebSite.SiteDb(), newMethodSetting);

                    fields.Paras = GetBindingPara(newMethodSetting);

                    return fields;
                }


            }
            else
            {

                var dataMethodSetting = call.WebSite.SiteDb().DataMethodSettings.Get(viewmodel.Id);

                if (dataMethodSetting == null)
                {
                    dataMethodSetting = Kooboo.Sites.DataSources.ScriptSourceManager.Get(call.WebSite.SiteDb(), viewmodel.Id);

                    dataMethodSetting.ParameterBinding = viewmodel.ParameterBinding;
                    dataMethodSetting.Description = viewmodel.Description;
                    dataMethodSetting.IsPublic = viewmodel.IsPublic;

                    call.WebSite.SiteDb().DataMethodSettings.AddOrUpdate(dataMethodSetting, call.Context.User.Id);

                    var fields = DataSourceHelper.GetFields(call.WebSite.SiteDb(), dataMethodSetting);
                    fields.Paras = GetBindingPara(dataMethodSetting);
                    return fields;
                }
                else
                {
                    dataMethodSetting.ParameterBinding = viewmodel.ParameterBinding;
                    dataMethodSetting.Description = viewmodel.Description;
                    dataMethodSetting.IsPublic = viewmodel.IsPublic;

                    call.WebSite.SiteDb().DataMethodSettings.AddOrUpdate(dataMethodSetting, call.Context.User.Id);

                    var fields = DataSourceHelper.GetFields(call.WebSite.SiteDb(), dataMethodSetting);
                    fields.Paras = GetBindingPara(dataMethodSetting);
                    return fields;
                }

            }

            return null;

        }

        private DataMethodViewModel UpdateViewModel(DataMethodViewModel model, SiteDb siteDb, IDataMethodSetting setting)
        {
            var typeInfo = DataSourceHelper.GetFields(siteDb, setting);
            model.IsGlobal = setting.IsGlobal;
            model.MethodName = setting.MethodName;
            model.ParameterBinding = setting.ParameterBinding;
            model.Description = setting.Description;
            model.Parameters = setting.Parameters;
            model.Id = setting.Id;
            model.OriginalMethodName = setting.OriginalMethodName;
            model.MethodSignatureHash = setting.MethodSignatureHash;
            model.DeclareType = setting.DeclareType;
            model.IsGlobal = setting.IsGlobal;
            model.ReturnType = setting.ReturnType;
            model.IsPagedResult = setting.IsPagedResult;

            model.ItemFields = typeInfo.ItemFields;
            model.Enumerable = typeInfo.Enumerable;
            model.IsPublic = setting.IsPublic;
            return model;
        }

        [Kooboo.Attributes.ReturnType(typeof(DataMethodSetting))]
        [Kooboo.Attributes.RequireParameters("id")]
        public override object Get(ApiCall call)
        {
            var datamethod = call.WebSite.SiteDb().DataMethodSettings.Get(call.ObjectId);
            if (datamethod == null)
            {
                datamethod = Kooboo.Data.GlobalDb.DataMethodSettings.Get(call.ObjectId);
            }

            if (datamethod == null)
            {
                datamethod = Kooboo.Sites.DataSources.ScriptSourceManager.Get(call.WebSite.SiteDb(), call.ObjectId);
            }

            if (datamethod != null)
            {
                SetDisplayName(datamethod, call);
                return datamethod;
            }
            return null;
        }

        private void SetDisplayName(DataMethodSetting setting, ApiCall call)
        {

            foreach (var item in setting.ParameterBinding)
            {
                var value = item.Value;
                value.DisplayName = item.Key;   // Data.Language.Hardcoded.GetValuexxxx(item.Key, call.Context); 
            }
        }

        public override bool IsUniqueName(ApiCall call)
        {
            string name = call.NameOrId;
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            name = name.ToLower();

            var all = call.WebSite.SiteDb().DataMethodSettings.All();

            foreach (var item in all)
            {
                if (item.MethodName.ToLower() == name)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetRelation(List<DataSourceViewModel> datasources, SiteDb siteDb)
        {
            foreach (var item in datasources)
            {
                foreach (var method in item.Methods)
                {
                    var viewdatamethod = siteDb.ViewDataMethods.Query.Where(o => o.MethodId == method.Id).SelectAll();

                    if (viewdatamethod != null && viewdatamethod.Count() > 0)
                    {
                        method.Relations.Add("View", viewdatamethod.Count());
                    }
                }
            }
        }

        public List<UsedByRelation> ShowRelation(ApiCall call)
        {
            var viewdatamethod = call.WebSite.SiteDb().ViewDataMethods.Query.Where(o => o.MethodId == call.ObjectId).SelectAll();


            string by = call.GetValue("by");
            if (string.IsNullOrEmpty(by))
            { return null; }

            byte consttype = ConstTypeContainer.GetConstType(by);

            var usedby = call.WebSite.SiteDb().Images.GetUsedBy(call.ObjectId).Where(o => o.ConstType == consttype).ToList();

            foreach (var item in usedby)
            {
                item.Url = call.WebSite.SiteDb().WebSite.BaseUrl(item.Url);
            }

            return usedby;
        }

        private Dictionary<string, string> GetBindingPara(DataMethodSetting setting)
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();
            foreach (var item in setting.ParameterBinding)
            {
                string name = item.Key;

                if (setting.IsKScript && name == Kooboo.Sites.DataSources.ScriptSourceManager.SampleResponseFieldName)
                {
                    continue;
                }

                string value = item.Value.Binding;
                if (value != null && value.Contains("{") && value.Contains("}"))
                {
                    paras.Add(name, value);
                }
            }
            return paras;
        }


        private List<DataSourceViewModel> FilterOut(List<DataSourceViewModel> input, WebSite site)
        {
            // filter out not used datasource. 
            List<DataSourceViewModel> result = new List<DataSourceViewModel>();
            foreach (var item in input)
            {
                if (item.DataSourceType == typeof(Kooboo.Sites.DataSources.Search))
                {
                    if (site.EnableFullTextSearch)
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
