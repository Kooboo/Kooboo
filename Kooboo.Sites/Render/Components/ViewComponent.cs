//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Sites.Service;
using Kooboo.Sites.Models;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Data.Models;

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

        public string StoreEngineName { get { return null;  } }

        public byte StoreConstType { get { return ConstObjectType.View;  } }

        public string DisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("View", Context); 
        }

        public List<ComponentInfo> AvaiableObjects(SiteDb sitedb)
        {
            List<ComponentInfo> Models = new List<ComponentInfo>();
            var allview = sitedb.Views.All();
            foreach (var item in allview)
            {
                ComponentInfo comp = new ComponentInfo();
                comp.Id = item.Id;
                comp.Name = item.Name;
                Models.Add(comp);
            }
            return Models;
        }


        public string Preview(SiteDb SiteDb, string NameOrId)
        {
            if (string.IsNullOrEmpty(NameOrId))
            {
                return null;
            }
            var view = SiteDb.Views.GetByNameOrId(NameOrId);
            return view != null ? view.Body : null;
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

                        if (result !=null)
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

            if (setting.Settings != null && setting.Settings.Count() > 0)
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
                    if (result is PagedResult)
                    { 
                        var pagedresult = result as  PagedResult; 
                 
                        dataResults[datemethod.AliasName] = pagedresult.DataList; 
                        dataResults[datemethod.AliasName + ".TotalPages"] = pagedresult.TotalPages;
                        List<int> PageNrs = new List<int>();
                        for (int i = 1; i <= pagedresult.TotalPages; i++)
                        {
                            PageNrs.Add(i); 
                        }
                        dataResults[datemethod.AliasName + ".CurrentPage"] = pagedresult.PageNumber;
                        dataResults[datemethod.AliasName + ".Pages"] = PageNrs;
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

            var options = RenderOptionHelper.GetViewOption(context, viewid); 
             
            if (options.RequireBindingInfo)
            {
                viewBody = DomService.ApplyKoobooId(viewBody); 
                renderplan = RenderEvaluator.Evaluate(viewBody, options); 
            }
            else
            { 
                renderplan = Cache.RenderPlan.GetOrAddRenderPlan(frontcontext.SiteDb, viewid, () => RenderEvaluator.Evaluate(viewBody, options));
            }
             

            returnstring += RenderHelper.Render(renderplan, frontcontext.RenderContext);

            if (dataResults.Count > 0)
            {
                context.DataContext.Pop();
            }


            if (setting.Settings != null && setting.Settings.Count() > 0)
            {
                context.DataContext.Pop();
            }
            

            frontcontext.AddLogEntry("view", view.Name, logstart, 200);

            frontcontext.ExecutingView = null;
            return Task.FromResult(returnstring);

        }

        private View CheckAlternativeView(View CurrentView, FrontContext Context)
        {
            string StrAlternative = Context.RenderContext.Request.QueryString.Get(SiteConstants.AlternativeViewQueryName);

            int alternativeid;
            if (!string.IsNullOrEmpty(StrAlternative) && int.TryParse(StrAlternative, out alternativeid))
            {
                var alterviewid = Cache.ViewInSamePosition.GetAlternaitiveViewId(alternativeid, CurrentView.Id);
                if (alterviewid != default(Guid))
                {
                    var alterview = Context.SiteDb.Views.Get(alterviewid);
                    if (alterview != null)
                    {
                        return alterview;
                    }
                }
            }

            if (Context.AltervativeViews != null)
            {
                foreach (var item in Context.AltervativeViews)
                {
                    var alterviewid = Cache.ViewInSamePosition.GetAlternaitiveViewId(item, CurrentView.Id);
                    if (alterviewid != default(Guid))
                    {
                        var alterview = Context.SiteDb.Views.Get(alterviewid);
                        if (alterview != null)
                        {
                            return alterview;
                        }
                    }
                }
            }

            return CurrentView;
        }
    }
}
