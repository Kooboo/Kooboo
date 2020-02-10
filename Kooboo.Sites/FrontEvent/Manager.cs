//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting;
using System.Linq;
using System.Collections.Generic;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Render.Components;
using System.Collections;
using System;
using KScript; 

namespace Kooboo.Sites.FrontEvent
{
    public static class Manager
    {

        public static Kooboo.Sites.Routing.Route RaiseRouteEvent(enumEventType eventtype, RenderContext context, Kooboo.Sites.Routing.Route route = null)
        {
            if (eventtype == enumEventType.RouteFinding)
            {
                RouteFinding finding = new RouteFinding(context);
                RaiseEvent(context, finding);
                return finding.Route;
            }
            else if (eventtype == enumEventType.RouteFound)
            {
                if (route != null)
                {
                    RouteFound found = new RouteFound(context, route);
                    RaiseEvent(context, found);
                    if (found.DataChange && found.Route != null)
                    {
                        return found.Route;
                    }
                }
            }
            else if (eventtype == enumEventType.RouteNotFound)
            {
                RouteNotFound notfound = new RouteNotFound(context);
                RaiseEvent(context, notfound);
                return notfound.Route;
            }

            return null;
        }

        public static Models.Page RaisePageEvent(enumEventType eventtype, RenderContext context, Models.Page page = null)
        {
            if (eventtype == enumEventType.PageFinding)
            {
                PageFinding finding = new PageFinding(context);
                RaiseEvent(context, finding);
                return finding.Page;
            }
            else if (eventtype == enumEventType.PageFound)
            {
                if (page != null)
                {
                    PageFound found = new PageFound(context, page);
                    RaiseEvent(context, found);
                    if (found.DataChange && found.Page != null)
                    {
                        return found.Page;
                    }
                }
            }
            //else if (eventtype == enumEventType.)
            //{
            //    PageNotFound notfound = new PageNotFound(context);
            //    RaiseEvent(context, notfound);
            //    return notfound.Page;
            //}
            return null;
        }

        public static Models.View RaiseViewEvent(enumEventType eventtype, RenderContext context, ComponentSetting setting, Models.View view = null)
        {
            if (eventtype == enumEventType.ViewFinding)
            {
                ViewFinding finding = new ViewFinding(context, setting);
                RaiseEvent(context, finding);
                return finding.View;
            }
            else if (eventtype == enumEventType.ViewFound)
            {
                if (view != null)
                {
                    ViewFound found = new ViewFound(context, setting, view);

                    RaiseEvent(context, found);

                    if (found.DataChange && found.View != null)
                    {
                        return found.View;
                    }
                }
            }
            else if (eventtype == enumEventType.ViewNotFound)
            {
                ViewNotFound notfound = new ViewNotFound(context, setting);
                RaiseEvent(context, notfound);
                return notfound.View;
            }
            return null;
        }



        public static void RaiseEvent(RenderContext context, IFrontEvent theevent)
        {
            var sitedb = context.WebSite.SiteDb(); 
            var list = sitedb.Rules.ListByEventType(theevent.EventType);

            if (list == null || list.Count() == 0)
            {
                return;
            } 
            RaiseEvent(context, theevent, list);
        }
         
        public static void RaiseEvent(RenderContext context, IFrontEvent theevent, List<Sites.Models.BusinessRule> rules)
        {
            var sitedb = context.WebSite.SiteDb();

            var engine = Kooboo.Sites.Scripting.Manager.GetJsEngine(context);
            var kcontext = context.GetItem<k>();
            kcontext.@event = theevent;

            foreach (var item in rules)
            {
                if (item.Rule != null)
                {
                    ExecuteRule(sitedb,   kcontext, theevent, item.Rule);
                }
            }

            kcontext.@event = null;
        }


        public static void ExecuteRule(SiteDb sitedb,   KScript.k kcontext, IFrontEvent theevent, Kooboo.Sites.Models.IFElseRule rule)
        {
            if (rule.Do != null && rule.Do.Count() > 0)
            {
                foreach (var item in rule.Do)
                {
                    var code = sitedb.Code.Get(item.CodeId);
                    if (code != null && !string.IsNullOrWhiteSpace(code.Body))
                    {
                        kcontext.config = new KDictionary(CopySetting(item.Setting));

                        var outputstring = Kooboo.Sites.Scripting.Manager.ExecuteCode(kcontext.RenderContext, code.Body, code.Id); 
                        
                        if (!string.IsNullOrEmpty(outputstring))
                        {
                            kcontext.RenderContext.Response.AppendString(outputstring); 
                        }
                        kcontext.config = null;
                    }
                }
            }

            if (rule.IF != null && rule.IF.Count() > 0)
            {
                var check = EvaluteCondition(theevent, rule.IF);

                if (check)
                {
                    if (rule.Then != null && rule.Then.Count() > 0)
                    {
                        foreach (var item in rule.Then)
                        {
                            ExecuteRule(sitedb,   kcontext, theevent, item);
                        }
                    }
                }
                else
                {
                    if (rule.Else != null && rule.Else.Count() > 0)
                    {
                        foreach (var item in rule.Else)
                        {
                            ExecuteRule(sitedb,   kcontext, theevent, item);
                        }
                    }
                }
            }

            else if (rule.Then != null && rule.Then.Count > 0)
            {

                foreach (var item in rule.Then)
                {
                    ExecuteRule(sitedb,   kcontext, theevent, item);
                } 
            }
        }

        public static bool EvaluteCondition(IFrontEvent theevent, List<Condition> conditions)
        {
            foreach (var item in conditions)
            {
                if (!FrontEvent.ConditionManager.Evaluate(theevent, item))
                {
                    return false;
                }
            }
            return true;
        }
         
 
        public static List<EventConditionSetting> GetConditionSetting(enumEventType eventype, RenderContext context)
        {
            var alltypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IFrontEvent));

            foreach (var item in alltypes)
            {

                if (System.Activator.CreateInstance(item) is IFrontEvent instance && instance.EventType == eventype)
                {
                    return instance.GetConditionSetting(context);
                }
            }

            return new List<EventConditionSetting>();
        }

        public static Dictionary<string, string> CopySetting(Dictionary<string, string> input)
        {   
            if (input == null || !input.Any())
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 
            }
            
            var comparer = StringComparer.OrdinalIgnoreCase;

            var newDictionary = new Dictionary<string, string>(input, comparer);

            return newDictionary; 
           
        }

    }
}

