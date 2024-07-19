//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Models;

namespace Kooboo.Web.ViewModel
{

    public class ViewItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public DateTime LastModified { get; set; }

        public string Preview { get; set; }

        public string RelativeUrl { get; set; }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public Dictionary<string, int> Relations { get; set; }

        public int DataSourceCount { get; set; }
    }

    public class ViewViewModel
    {
        public string Name { get; set; }

        public string Body { get; set; }

        public string DummyLayout { get; set; }

        public Dictionary<string, List<string>> Layouts = new Dictionary<string, List<string>>();

        public long Version { get; set; }

        public List<ContentProperty> PropDefines { get; set; } = new List<ContentProperty>();
    }


    public class ViewEditViewModel : IDiffChecker
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public List<ViewDataMethod> DataSources { get; set; }

        public List<ContentProperty> PropDefines { get; set; } = new List<ContentProperty>();

        public long Version { get; set; }
        public bool? EnableDiffChecker { get; set; }

        /// public List<FormBinding> FormBindings { get; set; }

    }

}
