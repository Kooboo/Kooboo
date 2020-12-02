using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kooboo.Sites.OAuth2
{
    public abstract class BaseOAuth2<T> : IOAuth2 where T : ISiteSetting
    {

        protected static readonly WebClient _webClient = new WebClient();

        public BaseOAuth2(RenderContext context)
        {
            Context = context;
            if (context.WebSite == null) throw new Exception("Not WebSite");
            Setting = context.WebSite.SiteDb().CoreSetting.GetSetting<T>();
            if (Setting == null) throw new Exception($"Not SiteSetting");
        }

        [Kooboo.Data.Attributes.KIgnore]
        public T Setting { get; set; }

        [Kooboo.Data.Attributes.KIgnore]
        public RenderContext Context { get; set; }

        public abstract string GetRedirectUrl(IDictionary<string, object> @params);

        [Kooboo.Data.Attributes.KIgnore]
        public abstract string Callback(IDictionary<string, object> query);

    }
}
