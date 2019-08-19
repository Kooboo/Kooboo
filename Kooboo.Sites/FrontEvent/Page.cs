//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
 
namespace Kooboo.Sites.FrontEvent
{
    public class PageFinding : IFrontEvent
    {
        public PageFinding()
        {

        }
        public PageFinding(RenderContext context)
        {
            this.Context = context;
        }

        [Attributes.SummaryIgnore]
        public RenderContext Context { get; set; }

        public string Url
        {
            get { return Context.Request.RelativeUrl; }
            set
            {
                var url = value;
                if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                { 
                    //TODO: check if it is the same host.... then treat it as relativeurl.

                    Context.Response.Redirect(302, url);
                    Context.Response.End = true;
                    return;
                }

                url = url.Replace("\\", "/");
                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }
                Context.Request.RelativeUrl = url;

            }
        }

        public string UserAgent
        {
            get
            {
                return this.Context.Request.Headers.Get("User-Agent");
            }
        }

        public string Culture
        {
            get { return Context.Culture; }

            set
            {
                var culture = value;
                if (Context.WebSite.Culture.ContainsKey(culture))
                {
                    Context.Culture = culture;
                }
            }
        }

        [Attributes.SummaryIgnore]
        public enumEventType EventType => enumEventType.PageFinding;

        private Page _page; 
        public Page Page
        {
            get { return _page;  }
            set { _page = value; DataChange = true;  }
        }
         
        public bool DataChange { get; set; }

        [Attributes.SummaryIgnore]
        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            result.Add(new EventConditionSetting() { Name = "Url" });
            result.Add(new EventConditionSetting() { Name = "Culture" });
            result.Add(new EventConditionSetting() { Name = "UserAgent" });
            return result;
        }
    }

    public class PageFound : IFrontEvent
    {
        public PageFound()
        {

        }
        public PageFound(RenderContext context, Page page)
        {
            this._page = page;
            this.Context = context;
        }

        [Attributes.SummaryIgnore]
        public RenderContext Context { get; set; }

        public string Url
        {
            get { return Context.Request.RelativeUrl; }
            set
            {
                var url = value;
                if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                {
                    //TODO: check if it is the same host.... then treat it as relativeurl.

                    Context.Response.Redirect(302, url);
                    Context.Response.End = true;
                    return;
                }

                url = url.Replace("\\", "/");
                if (!url.StartsWith("/"))
                {
                    url = "/" + url;
                }
                Context.Request.RelativeUrl = url;

            }
        }

        public string UserAgent
        {
            get
            {
                return this.Context.Request.Headers.Get("User-Agent");
            }
        }

        public string Culture
        {
            get { return Context.Culture; }

            set
            {
                var culture = value;
                if (Context.WebSite.Culture.ContainsKey(culture))
                {
                    Context.Culture = culture;
                }
            }
        }

        [Attributes.SummaryIgnore]
        public enumEventType EventType => enumEventType.PageFound;

        private Page _page;
        public Page Page
        {
            get { return _page; }
            set { _page = value; DataChange = true; }
        }

        public bool DataChange { get; set; }
         
        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            result.Add(new EventConditionSetting() { Name = "Url" });
            result.Add(new EventConditionSetting() { Name = "Culture" });
            result.Add(new EventConditionSetting() { Name = "UserAgent" });

            result.Add(new EventConditionSetting() { Name = "Page.Name" });
            result.Add(new EventConditionSetting() { Name = "Page.DefaultStart", ControlType = Data.ControlType.CheckBox, DataType = typeof(bool) });
            result.Add(new EventConditionSetting() { Name = "Page.Body" });

            Dictionary<string, string> pagenames = new Dictionary<string, string>();
            foreach (var item in context.WebSite.SiteDb().Pages.All())
            {
                var url = Sites.Service.ObjectService.GetObjectRelativeUrl(context.WebSite.SiteDb(), item);
                pagenames.Add(item.Id.ToString(), url);
            }

            result.Add(new EventConditionSetting() { Name = "Page.Id", ControlType = Data.ControlType.Selection, DataType = typeof(Guid), SelectionValues = pagenames });
            return result;
        }
    }
     
    //public class PageNotFound : IFrontEvent
    //{
    //    public PageNotFound()
    //    {

    //    }

    //    public PageNotFound(RenderContext context)
    //    {
    //        this.Context = context;
    //    }

    //    [Attributes.SummaryIgnore]
    //    public RenderContext Context { get; set; }

    //    public string Url
    //    {
    //        get { return Context.Request.RelativeUrl; }
    //        set
    //        {
    //            var url = value;
    //            if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
    //            {
    //                //TODO: check if it is the same host.... then treat it as relativeurl.

    //                Context.Response.Redirect(302, url);
    //                Context.Response.End = true;
    //                return;
    //            }

    //            url = url.Replace("\\", "/");
    //            if (!url.StartsWith("/"))
    //            {
    //                url = "/" + url;
    //            }
    //            Context.Request.RelativeUrl = url;

    //        }
    //    }

    //    public string UserAgent
    //    {
    //        get
    //        {
    //            return this.Context.Request.Headers.Get("User-Agent");
    //        }
    //    }

    //    public string Culture
    //    {
    //        get { return Context.Culture; }

    //        set
    //        {
    //            var culture = value;
    //            if (Context.WebSite.Culture.ContainsKey(culture))
    //            {
    //                Context.Culture = culture;
    //            }
    //        }
    //    }

    //    [Attributes.SummaryIgnore]
    //    public enumEventType EventType => enumEventType.PageNotFound;

    //    private Page _page;
    //    public Page Page
    //    {
    //        get { return _page; }
    //        set { _page = value; DataChange = true; }
    //    }

    //    public bool DataChange { get; set; }

    //    [Attributes.SummaryIgnore]
    //    public List<EventConditionSetting> GetConditionSetting(RenderContext context)
    //    {
    //        List<EventConditionSetting> result = new List<EventConditionSetting>();
    //        result.Add(new EventConditionSetting() { Name = "Url" });
    //        result.Add(new EventConditionSetting() { Name = "Culture" });
    //        result.Add(new EventConditionSetting() { Name = "UserAgent" });
    //        return result;
    //    }
    //}

}
