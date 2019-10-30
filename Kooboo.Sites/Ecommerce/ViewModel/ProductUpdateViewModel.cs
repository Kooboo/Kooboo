//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class ProductUpdateViewModel
    {
        public List<Guid> Categories { get; set; }

        public string Id { get; set; }

        public string UserKey { get; set; }

        public string ProductTypeId { get; set; }

        public bool Online { get; set; }

        private Dictionary<string, Dictionary<string, string>> _values;

        /// <summary>
        /// {enus:{title:"title",summary:"summary"}}
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Values
        {
            get { return _values ?? (_values = new Dictionary<string, Dictionary<string, string>>()); }
            set { _values = value; }
        }

        public List<ProductVariants> Variants { get; set; }
    }
}