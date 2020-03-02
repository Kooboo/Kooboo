//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class Category : MultipleLanguageObject
    {
        public Category()
        {
            this.ConstType = ConstObjectType.Cateogry;
        }
        public Guid ParentId { get; set; }

        private Guid _Id;
        public override Guid Id
        {
            get
            {
                if (_Id == default(Guid) && !string.IsNullOrWhiteSpace(this.UserKey))
                {
                    _Id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.UserKey);
                }
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        private string _userkey;
        public string UserKey
        {
            get
            {
                return _userkey;
            }
            set
            {
                _userkey = value;
                _Id = default(Guid);
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Name + this.ParentId.ToString();

            if (this.Values != null)
            {
                foreach (var item in this.Values)
                {
                    unique += item.Key + item.Value;
                }
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
