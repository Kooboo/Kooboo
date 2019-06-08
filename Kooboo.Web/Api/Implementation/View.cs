//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Definition;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.Areas.Admin.ViewModels;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class ViewApi : SiteObjectApi<View>
    {
        public override object Get(ApiCall call)
        {
            var view = call.WebSite.SiteDb().Views.Get(call.ObjectId);
            if (view == null)
            {
                view = new View();
                view.Body = DefaultView();
            }

            ViewViewModel viewmodel = new ViewViewModel() { Name = view.Name, Body = view.Body };

            viewmodel.DummyLayout = GetDummary(call.WebSite);

            var layouts = call.WebSite.SiteDb().Layouts.All();
            foreach (var item in layouts)
            {
                viewmodel.Layouts.Add(item.Name, GetLayoutPositions(item));
            }
            return viewmodel;
        }

        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            List<ViewItem> result = new List<ViewItem>();
            string baseurl = call.WebSite.BaseUrl();

            var allviews = sitedb.Views.All(true).OrderBy(o => o.Name);
            int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.Views.StoreName);

            foreach (var item in allviews)
            {
                string url = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, item);
                string previewurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url);
                ViewItem model = new ViewItem() { Name = item.Name, Id = item.Id, LastModified = item.LastModified, Preview = previewurl };
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storenamehash;
                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Views.GetUsedBy(item.Id));
                model.DataSourceCount = sitedb.ViewDataMethods.Query.Where(o => o.ViewId == item.Id).Count();
                result.Add(model);
            }
            return result.ToList<object>();
        }

        private string GetDummary(WebSite site)
        {
            string body = @"<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""utf-8""><meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1""><meta name=""viewport"" content=""width=device-width,initial-scale=1,maximum-scale=1, user-scalable=no"">
         </head>
         <body> <div k-placeholder=""Main"" ></div>
          </body>
          </html>";

            body = body.Replace("<head>", $"<head><base href=\"{site.BaseUrl()}\"/>");
            return body;
        }

        public List<ViewDataMethod> ViewMethods(ApiCall call)
        {
            var methods = call.WebSite.SiteDb().ViewDataMethods.Query.Where(o => o.ViewId == call.ObjectId).SelectAll();
            return methods;
        }

        internal List<string> GetLayoutPositions(Layout layout)
        {
            var dom = Kooboo.Dom.DomParser.CreateDom(layout.Body);
            List<string> positionames = new List<string>();
            GetPosition(dom.documentElement, ref positionames);

            return positionames;
        }

        private void GetPosition(Dom.Element element, ref List<string> positions)
        {
            foreach (var item in element.attributes)
            {
                var lower = item.name.ToLower();
                if (lower == "tal-placeholder" || lower == "position" || lower == "placeholder" || lower == "k-placeholder")
                {
                    string value = item.value;
                    if (!positions.Contains(value))
                    {
                        positions.Add(value);
                    }
                }
            }

            foreach (var item in element.childNodes.item)
            {
                if (item is Dom.Element)
                {
                    GetPosition(item as Dom.Element, ref positions);
                }
            }
        }

        private string DefaultView()
        {
            string defaultview = @"<div class=""jumbotron"">
    <h2>Welcome!</h2>
    <p>Welcome to Kooboo CMS</p>
    <p>Cras justo odio, dapibus ac facilisis in, egestas eget quam.Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.</p>
    <p class=""text-center"">
        <a class=""btn blue btn-lg"" href=""setup-step-1.shtml"" role=""button"">Get start!</a>
    </p>
</div>";
            return defaultview;

        }

        [Kooboo.Attributes.RequireModel(typeof(ViewModel.ViewEditViewModel))]
        public override Guid Post(ApiCall call)
        {
            var model = call.Context.Request.Model as ViewEditViewModel;
            View view;
            if (model.Id == default(Guid))
            {
                view = new View() { Name = model.Name, Body = model.Body };
                call.WebSite.SiteDb().Views.AddOrUpdate(view, call.Context.User.Id);
            }
            else
            {
                view = call.WebSite.SiteDb().Views.Get(model.Id);
                view.Body = model.Body;
                call.WebSite.SiteDb().Views.AddOrUpdate(view, call.Context.User.Id);
            }

            if (model.DataSources == null)
            {
                model.DataSources = new List<ViewDataMethod>();
            }

            call.WebSite.SiteDb().Views.UpdateDataSources(view.Id, model.DataSources, call.Context.User.Id);

            Kooboo.Sites.Service.CleanerService.CleanDataMethod(call.WebSite.SiteDb());

            return view.Id;
        }
                     
        
        
        public Dictionary<string, ComparerModel[]> CompareType(ApiCall call)
        {
            var types = Data.Helper.DataTypeHelper.GetDataTypeCompareModel();

            Dictionary<string, ComparerModel[]> result = new Dictionary<string, ComparerModel[]>();

            foreach (var item in types)
            {
                var clrtype = Kooboo.Data.Helper.DataTypeHelper.ToClrType(item.Key);
                var name = clrtype.FullName;
                result[name] = item.Value;
            }


            var kooboodatatypes = Data.Helper.DataTypeHelper.GetDataTypeCompareModel();

            foreach (var item in kooboodatatypes)
            {
                var name = Enum.GetName(typeof(Kooboo.Data.Definition.DataTypes), item.Key);

                result[name] = item.Value;   
            }        
            return result;
        }

        public List<ViewDataSourceViewModel> DataMethod(ApiCall call)
        {
            Guid viewId = call.ObjectId;
            var dataSources = call.WebSite.SiteDb().ViewDataMethods.Query.Where(it => it.ViewId == viewId).SelectAll();

            var models = new List<ViewDataSourceViewModel>();
            foreach (var dataSource in dataSources)
            {
                models.AddRange(ViewDataSourceViewModel.Create(dataSource));
            }

            return models;
        }

        [Kooboo.Attributes.RequireParameters("id", "name")]
        public ViewItem Copy(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var view = sitedb.Views.Get(call.ObjectId);
            if (view != null)
            {
                var newview = Lib.Serializer.Copy.DeepCopy<View>(view);
                newview.CreationDate = DateTime.UtcNow;
                newview.LastModified = DateTime.UtcNow;
                newview.Name = call.GetValue("name");
                sitedb.Views.AddOrUpdate(newview, call.Context.User.Id);

                // now copy the viewdatemethod. 
                var datamethods = sitedb.ViewDataMethods.Query.Where(o => o.ViewId == view.Id).SelectAll();
                foreach (var method in datamethods)
                {
                    var copymethod = Lib.Serializer.Copy.DeepCopy<ViewDataMethod>(method);
                    copymethod.ViewId = newview.Id;
                    copymethod.Id = default(Guid);

                    // check to see if datamethod is private.  
                    var datamethod = sitedb.DataMethodSettings.Get(copymethod.MethodId);
                    if (datamethod != null && !datamethod.IsPublic)
                    {
                        var copydatamethod = Lib.Serializer.Copy.DeepCopy<DataMethodSetting>(datamethod);
                        copydatamethod.MethodName = System.Guid.NewGuid().ToString();
                        copydatamethod.Id = default(Guid);
                        sitedb.DataMethodSettings.AddOrUpdate(copydatamethod);
                        copymethod.MethodId = copydatamethod.Id;
                    }

                    sitedb.ViewDataMethods.AddOrUpdate(copymethod, call.Context.User.Id);
                }


                string baseurl = call.WebSite.BaseUrl();

                int storenamehash = Lib.Security.Hash.ComputeInt(sitedb.Views.StoreName);

                string url = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, newview);
                string previewurl = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url);

                ViewItem model = new ViewItem() { Name = newview.Name, Id = newview.Id, LastModified = newview.LastModified, Preview = previewurl };
                model.KeyHash = Sites.Service.LogService.GetKeyHash(newview.Id);
                model.StoreNameHash = storenamehash;

                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Views.GetUsedBy(newview.Id));
                model.DataSourceCount = sitedb.ViewDataMethods.Query.Where(o => o.ViewId == newview.Id).Count();

                return model;

            }
            return null;
        }

    }
}
