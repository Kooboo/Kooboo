//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Render.Components;
using Kooboo.Sites.Events.FrontFlow;
using Kooboo.Sites.Repository;
using Kooboo.Sites.FrontEvent; 
  
namespace Kooboo.Sites.FrontEvent
{
    public class ViewFinding : IFrontEvent
    { 
        public ViewFinding()
        {

        }
        public ViewFinding(RenderContext context, ComponentSetting comsetting)
        {
            this.Context = context;
            this.componentSetting = comsetting; 
        } 

        public string Name
        {
            get { return this.componentSetting.NameOrId;  }
        }
        
        public Dictionary<string, string> setting
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                if (this.componentSetting.Settings !=null)
                {
                    foreach (var item in this.componentSetting.Settings)
                    {
                        result.Add(item.Key, item.Value); 
                    }
                }
                return result;  
            }
        }

       
       private ComponentSetting componentSetting { get; set; }

        public RenderContext Context { get; set; }
         
        public View View { get; set; }
    
        public bool DataChange
        {
            get; set;
        }

        public enumEventType EventType =>  enumEventType.ViewFinding;

        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            result.Add(new EventConditionSetting() { Name = "NameOrId" });
            return result;
        }
    }

    public class ViewFound : IFrontEvent
    {
        public ViewFound()
        {

        }
 
        public ViewFound(RenderContext context, ComponentSetting comsetting,  View View)
        {
            this.Context = context;
            this._view = View;
            this.componentSetting = comsetting; 
        }

        public string Name
        {
            get { return this.componentSetting.NameOrId; }
        }

        public Dictionary<string, string> setting
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                if (this.componentSetting.Settings != null)
                {
                    foreach (var item in this.componentSetting.Settings)
                    {
                        result.Add(item.Key, item.Value);
                    }
                }
                return result;
            }
        }


        private ComponentSetting componentSetting { get; set; }

        public RenderContext Context { get; set; }


        private View _view; 
        public View View {
            get { return _view;  }
            set
            {
                _view = value;
                DataChange = true; 
            }
        }

        public bool DataChange
        {
            get; set;
        }

        public enumEventType EventType => enumEventType.ViewFound;

        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            var sitedb = context.WebSite.SiteDb();

            result.AddRange(FrontEventHelper.GetViewSetting(sitedb));
            return result;
        }
    }

    public class ViewNotFound : IFrontEvent
    {
        public ViewNotFound() { }

        public ViewNotFound(RenderContext context, ComponentSetting ComSettings)
        {
            this.Context = context;
            this.componentSetting = ComSettings; 
        }

        public string Name
        {
            get { return this.componentSetting.NameOrId; }
        }
           
        private ComponentSetting componentSetting { get; set; }

        public RenderContext Context { get; set; }
 
        public View View
        {
            get;set;
        }

        public bool DataChange
        {
            get; set;
        }

        public enumEventType EventType => enumEventType.ViewNotFound; 

        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            result.Add(new EventConditionSetting() { Name = "NameOrId" });
            return result;
        }
    }
    
    public class ViewEventHelper
    {
        public static List<EventConditionSetting> GetViewSetting(SiteDb siteDb)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();

            Dictionary<string, string> viewnames = new Dictionary<string, string>();
            foreach (var item in siteDb.Views.All())
            {
                viewnames.Add(item.Id.ToString(), item.Name);
            }

            result.Add(new EventConditionSetting() { Name = "View.Id", ControlType = Data.ControlType.Selection, DataType = typeof(Guid), SelectionValues = viewnames });

            result.Add(new EventConditionSetting() { Name = "View.Name" });

            return result;
        }
    }

}
