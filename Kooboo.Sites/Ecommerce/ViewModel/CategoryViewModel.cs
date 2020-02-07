//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using System;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class CategoryViewModel
    {
        public CategoryViewModel(Category cat, RenderContext context)
        {
            this.Id = cat.Id;
            this.ParentId = cat.ParentId;
            this.name = cat.Name;
            var value = cat.GetValue(context.Culture);
            if (value != null)
            {
                this.DisplayName = value.ToString();
            }
        }

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string name { get; set; }

        public string DisplayName { get; set; } 
        
    }
}
