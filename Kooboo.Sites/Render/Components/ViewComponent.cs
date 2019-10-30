//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Components
{
    public class ViewComponent : IComponent
    {
        public Dictionary<string, string> Setttings
        {
            get
            {
                return null;
            }
        }

        public string TagName
        {
            get { return "View"; }
        }

        public bool IsRegularHtmlTag { get { return false; } }

        public string StoreEngineName { get { return null; } }

        public byte StoreConstType { get { return ConstObjectType.View; } }

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("View", context);
        }

        public List<ComponentInfo> AvaiableObjects(SiteDb sitedb)
        {
            List<ComponentInfo> models = new List<ComponentInfo>();
            var allview = sitedb.Views.All();
            foreach (var item in allview)
            {
                ComponentInfo comp = new ComponentInfo {Id = item.Id, Name = item.Name};
                models.Add(comp);
            }
            return models;
        }

        public string Preview(SiteDb siteDb, string nameOrId)
        {
            if (string.IsNullOrEmpty(nameOrId))
            {
                return null;
            }
            var view = siteDb.Views.GetByNameOrId(nameOrId);
            return view?.Body;
        }

        public Task<string> RenderAsync(RenderContext context, ComponentSetting setting)
        {
            var frontcontext = context.GetItem<FrontContext>();
            DateTime logstart = DateTime.UtcNow;

            View view = null;

            if (context.WebSite.EnableFrontEvents && context.IsSiteBinding)
            {
                view = Kooboo.Sites.FrontEvent.Manager.RaiseViewEvent(FrontEvent.enumEventType.ViewFinding, context, setting);

                if (view == null)
                {
                    view = context.WebSite.SiteDb().Views.GetByNameOrId(setting.NameOrId);
                    if (view != null)
                    {
                        var result = FrontEvent.Manager.RaiseViewEvent(FrontEvent.enumEventType.ViewFound, context, setting, view);

                        if (result != null)
                        {
                            view = result;
                        }
                    }
                    else
                    {
                        view = FrontEvent.Manager.RaiseViewEvent(FrontEvent.enumEventType.ViewNotFound, context, setting);
                    }
                }
            }

            if (view == null)
            {
                view = context.WebSite.SiteDb().Views.GetByNameOrId(setting.NameOrId);
            }

            if (view == null)
            {
                frontcontext.AddLogEntry("view", "", logstart, 404);
                return Task.FromResult(string.Empty);
            }

            view = CheckAlternativeView(view, frontcontext);

            frontcontext.ExecutingView = view;

            var dataResults = new Dictionary<string, object>();
            string viewBody = null;
            frontcontext.Views.Add(view);

            if (setting.Settings != null && setting.Settings.Any())
            {
                context.DataContext.Push(setting.Settings);
            }

            var allviewdatamethods = Cache.SiteObjectCache<ViewDataMethod>.List(frontcontext.SiteDb).Where(o => o.ViewId == view.Id).ToList();

            foreach (var datemethod in allviewdatamethods)
            {
                frontcontext.ViewDataMethods.Add(datemethod);

                var result = DataSources.DataMethodExecutor.ExecuteViewDataMethod(frontcontext, datemethod);

                if (result != null)
                {
                    if (result is PagedResult pagedresult)
                    {
                        dataResults[datemethod.AliasName] = pagedresult.DataList;
                        dataResults[datemethod.AliasName + ".TotalPages"] = pagedresult.TotalPages;
                        List<int> pageNrs = new List<int>();
                        for (int i = 1; i <= pagedresult.TotalPages; i++)
                        {
                            pageNrs.Add(i);
                        }
                        dataResults[datemethod.AliasName + ".CurrentPage"] = pagedresult.PageNumber;
                        dataResults[datemethod.AliasName + ".Pages"] = pageNrs;
                    }
                    else
                    {
                        //if (result is DataMethodResult)
                        //{
                        //    var methodresult = result as DataMethodResult;
                        //    if (methodresult.Value is PagedResult)
                        //    {
                        //        var pagedresult = methodresult.Value as PagedResult;

                        //        dataResults[datemethod.AliasName + ".TotalPages"] = pagedresult.TotalPages;
                        //        List<int> PageNrs = new List<int>();
                        //        for (int i = 1; i <= pagedresult.TotalPages; i++)
                        //        {
                        //            PageNrs.Add(i);
                        //        }
                        //        dataResults[datemethod.AliasName + ".CurrentPage"] = pagedresult.PageNumber;
                        //        dataResults[datemethod.AliasName + ".Pages"] = PageNrs;
                        //    }
                        //}

                        dataResults[datemethod.AliasName] = result;
                    }
                }
            }

            if (dataResults.Count > 0)
            {
                context.DataContext.Push(dataResults);
            }

            viewBody = view.Body;

            Guid viewid = view.Id;

            List<IRenderTask> renderplan;
            string returnstring = string.Empty;

            EvaluatorOption options = new EvaluatorOption
            {
                RenderUrl = true, RenderHeader = false, OwnerObjectId = view.Id
            };

            if (frontcontext.RenderContext.Request.Channel == RequestChannel.InlineDesign)
            {
                viewBody = DomService.ApplyKoobooId(viewBody);
                options.RequireBindingInfo = true;
                renderplan = RenderEvaluator.Evaluate(viewBody, options);
            }
            else
            {
                renderplan = Cache.RenderPlan.GetOrAddRenderPlan(frontcontext.SiteDb, viewid, () => RenderEvaluator.Evaluate(viewBody, options));
            }

            returnstring += renderplan.Render(frontcontext.RenderContext);

            if (dataResults.Count > 0)
            {
                context.DataContext.Pop();
            }

            if (setting.Settings != null && setting.Settings.Any())
            {
                context.DataContext.Pop();
            }

            frontcontext.AddLogEntry("view", view.Name, logstart, 200);

            frontcontext.ExecutingView = null;
            return Task.FromResult(returnstring);
        }

        private View CheckAlternativeView(View currentView, FrontContext context)
        {
            string strAlternative = context.RenderContext.Request.QueryString.Get(SiteConstants.AlternativeViewQueryName);

            if (!string.IsNullOrEmpty(strAlternative) && int.TryParse(strAlternative, out var alternativeid))
            {
                var alterviewid = Cache.ViewInSamePosition.GetAlternaitiveViewId(alternativeid, currentView.Id);
                if (alterviewid != default(Guid))
                {
                    var alterview = context.SiteDb.Views.Get(alterviewid);
                    if (alterview != null)
                    {
                        return alterview;
                    }
                }
            }

            if (context.AltervativeViews != null)
            {
                foreach (var item in context.AltervativeViews)
                {
                    var alterviewid = Cache.ViewInSamePosition.GetAlternaitiveViewId(item, currentView.Id);
                    if (alterviewid != default(Guid))
                    {
                        var alterview = context.SiteDb.Views.Get(alterviewid);
                        if (alterview != null)
                        {
                            return alterview;
                        }
                    }
                }
            }

            return currentView;
        }
    }
}