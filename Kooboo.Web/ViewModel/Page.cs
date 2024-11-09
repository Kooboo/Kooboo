//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Sites.Render.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Kooboo.Web.ViewModel
{
    public class PageListViewModel
    {
        public PageListViewModel()
        {
            this.Layouts = new List<Layout>();
            this.Pages = new List<PageViewModel>();
        }
        public string BaseUrl { get; set; }

        public List<Layout> Layouts { get; set; }

        public List<PageViewModel> Pages { get; set; }
    }

    public class PageViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public int Warning { get; set; }

        public int Linked { get; set; }

        public int PageView { get; set; }

        public Guid LayoutId { get; set; }

        public bool Online { get; set; }

        public DateTime LastModified { get; set; }

        public string Path { get; set; }

        public string PreviewUrl { get; set; }

        public string InlineUrl { get; set; }

        public bool StartPage { get; set; }

        public Dictionary<string, int> Relations { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        [JsonConverter(typeof(StringEnumConverter))]
        public PageType Type { get; set; }

        public bool HasParameter { get; set; }
    }

    public class PageEditViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string UrlPath { get; set; }

        public string LayoutName { get; set; }

        public Guid LayoutId { get; set; }

        public Dictionary<string, List<ComponentSetting>> PlaceholderContents { get; set; }

        public Dictionary<string, string> ContentTitle { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public string Body { get; set; }

        public string DesignConfig { get; set; }

        public bool Published { get; set; } = true;

        public List<HtmlMeta> Metas { get; set; } = new List<HtmlMeta>();

        public List<string> MetaBindings { get; set; } = new List<string>();

        public List<string> UrlParamsBindings { get; set; } = new List<string>();

        public string CustomHeader { get; set; }
        public HashSet<string> Scripts { get; set; } = new HashSet<string>();

        public HashSet<string> Styles { get; set; } = new HashSet<string>();

        [JsonConverter(typeof(StringEnumConverter))]
        public PageType Type { get; set; }

        public bool EnableCache { get; set; }
        public bool DisableUnocss { get; set; }

        public bool CacheByVersion { get; set; }

        public int CacheMinutes { get; set; }

        public string CacheQueryKeys { get; set; }

        public string BaseUrl { get; set; }

        public long Version { get; set; }

        public string PreviewUrl { get; set; }
    }

    public class PageUpdateViewModel : IDiffChecker
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string LayoutName { get; set; }

        public string UrlPath { get; set; }

        public Dictionary<string, string> ContentTitle { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public string Body { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PageType? Type { get; set; }

        public string DesignConfig { get; set; }

        public bool Published { get; set; }

        public List<HtmlMeta> Metas { get; set; } = new List<HtmlMeta>();

        public HashSet<string> Scripts { get; set; } = new HashSet<string>();

        public HashSet<string> Styles { get; set; } = new HashSet<string>();

        public string CustomHeader { get; set; }
        public bool EnableCache { get; set; }
        public bool DisableUnocss { get; set; }

        public bool CacheByVersion { get; set; }

        public int CacheMinutes { get; set; }

        public string CacheQueryKeys { get; set; }
        public long Version { get; set; }
        public bool? EnableDiffChecker { get; set; }
    }

    public class PageDefaultRouteViewModel
    {
        public Guid StartPage { get; set; }
        public Guid NotFound { get; set; }
        public Guid Error { get; set; }
    }
}
