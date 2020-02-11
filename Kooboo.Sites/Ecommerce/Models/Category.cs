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
         
        private Guid _id;
        [Kooboo.Attributes.SummaryIgnore]
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        string unique = this.Name; 
                         
                        if (this.ParentId == default(Guid))
                        {
                            unique += this.ParentId.ToString(); 
                        }

                        unique += this.ConstType.ToString(); 
                                                         
                        _id = Data.IDGenerator.GetId(unique);
                    }
                    else
                    {
                        _id = System.Guid.NewGuid();
                    }
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string UserKey { get; set; }

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
