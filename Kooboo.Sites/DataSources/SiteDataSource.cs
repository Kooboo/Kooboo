//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Render;
namespace Kooboo.Sites.DataSources
{ 
    public abstract class SiteDataSource : IDataSource
    {
        public FrontContext Context { get; internal set; }
    }
}
