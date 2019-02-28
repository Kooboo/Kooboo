//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
 
namespace Kooboo.Sites.DataSources.Content
{
    public class ContentListParams
    { 
        public Guid FolderId { get; set; }

        private List<FilterDefinition> _filter; 
        public List<FilterDefinition> Filters {
            get
            {
                if (this._filter == null)
                {
                    this._filter = new List<FilterDefinition>(); 
                }
                return this._filter; 
            }
            set
            {
                this._filter = value; 
            } 
        }

        public string SortField { get; set; }
         
        public int Limit { get; set; } = 20;
         
        public bool IsAscending { get; set; }
         
        public bool EnablePaging { get; set; } = false; 
         
        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }

    public class ContentGetParams
    {
        public Guid FolderId { get; set; } 
        public List<FilterDefinition> Filters { get; set; } 
    }
     
}
