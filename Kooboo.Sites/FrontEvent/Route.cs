//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Routing;
using System.Collections.Generic;

namespace Kooboo.Sites.FrontEvent
{
    public class RouteFinding : IFrontEvent
    {
        public RouteFinding()
        {

        }

        public RouteFinding(RenderContext context)
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
                    Context.Response.Redirect(302, url);
                    Context.Response.End = true;
                }
                else
                {
                    url = url.Replace("\\", "/");
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }
                    Context.Request.RelativeUrl = url;
                }
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

        private Route _route;
        public Route Route
        {
            get { return _route; }
            set
            {
                _route = value;
                DataChange = true;
            }
        }

        [Attributes.SummaryIgnore]
        public enumEventType EventType => enumEventType.RouteFinding;

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

    public class RouteFound : IFrontEvent
    {
        public RouteFound()
        {

        }
        public RouteFound(RenderContext context, Route route)
        {
            this._route = route;
            this.Context = context;
        }

        public RenderContext Context { get; set; }

        public string Url
        {
            get { return Context.Request.RelativeUrl; }
            set
            {
                var url = value;
                if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                {
                    Context.Response.Redirect(302, url);
                    Context.Response.End = true;
                }
                else
                {
                    url = url.Replace("\\", "/");
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }
                    Context.Request.RelativeUrl = url;
                }
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

        private Route _route;
        public Route Route
        {
            get { return _route; }
            set
            {
                _route = value;
                DataChange = true;
            }
        }

        public bool DataChange
        {
            get; set;
        }

        public enumEventType EventType => enumEventType.RouteFound;

        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            result.Add(new EventConditionSetting() { Name = "Url" });
            result.Add(new EventConditionSetting() { Name = "Culture" });
            result.Add(new EventConditionSetting() { Name = "UserAgent" });

            Dictionary<string, string> objecttypes = new Dictionary<string, string>();

            foreach (var item in ConstTypeContainer.ByteTypes)
            {
                if (Attributes.AttributeHelper.IsRoutable(item.Value))
                {
                    objecttypes.Add(item.Key.ToString(), item.Value.Name);
                }
            }
              
            result.Add(new EventConditionSetting() { Name = "Route.DestinationConstType", ControlType = Data.ControlType.Selection, SelectionValues = objecttypes });

            return result;
        }
    }

    public class RouteNotFound : IFrontEvent
    {
        public RouteNotFound()
        {

        }
        public RouteNotFound(RenderContext context)
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
                    Context.Response.Redirect(302, url);
                    Context.Response.End = true;
                }
                else
                {
                    url = url.Replace("\\", "/");
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }
                    Context.Request.RelativeUrl = url;
                }
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
        public Route Route
        {
            get; set;
        }


        public enumEventType EventType => enumEventType.RouteNotFound;

        public bool DataChange { get; set; }

        public List<EventConditionSetting> GetConditionSetting(RenderContext context)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();
            result.Add(new EventConditionSetting() { Name = "Url" });
            result.Add(new EventConditionSetting() { Name = "Culture" });
            result.Add(new EventConditionSetting() { Name = "UserAgent" });

            return result;
        }
    }
}
