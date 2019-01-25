//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.DataSources
{
   public class Search: SiteDataSource
    {
        public List<SearchResult> FindAll(string keyword)
        { 
            var sitedb = this.Context.WebSite.SiteDb(); 
            if (string.IsNullOrEmpty(keyword))
            {
                return new List<SearchResult>(); 
            }  
            keyword = System.Net.WebUtility.UrlDecode(keyword);  

            string tag = "style='font-weight:bold;'";  
            return  sitedb.SearchIndex.Search(keyword, 99999, tag, this.Context.RenderContext);  
        }
         
        [Kooboo.Attributes.ReturnType(typeof(List<SearchResult>))]
        public  PagedResult PagedResult(string keyword, int PageSize, int PageNumber, string HighLightAttr)
        {  
            var sitedb = this.Context.WebSite.SiteDb();
            if (string.IsNullOrEmpty(keyword))
            {
                return new Data.Models.PagedResult(); 
            }

            keyword = System.Net.WebUtility.UrlDecode(keyword); 

            return sitedb.SearchIndex.SearchWithPaging(keyword, PageSize, PageNumber, HighLightAttr, this.Context.RenderContext); 
        }
                         
        [Kooboo.Attributes.ReturnType(typeof(List<SearchResult>))]
        public PagedResult ByFolder(List<Guid> FolderId, string keyword, int PageSize, int PageNumber, string HighLightAttr)
        {
         
            var sitedb = this.Context.WebSite.SiteDb();
            if (string.IsNullOrEmpty(keyword))
            {
                return new Data.Models.PagedResult();
            }

            keyword = System.Net.WebUtility.UrlDecode(keyword);
  
            return sitedb.SearchIndex.SearchByFolders(FolderId, keyword, PageSize, PageNumber,  HighLightAttr, this.Context.RenderContext);
        }
    } 
}
