//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Contents.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    [Kooboo.Attributes.NameAsID]
    public class ContentType : Kooboo.Sites.Models.CoreObject
    {
        public ContentType()
        {
            this.ConstType = ConstObjectType.ContentType; 
        }
        public bool IsNested { get; set; }

        private List<ContentProperty> _properties;
        public List<ContentProperty> Properties
        {
            get
            {
                if (_properties == null)
                { _properties = new List<ContentProperty>(); }
                return _properties;
            }
            set
            { _properties = value; }
        }

        public ContentProperty GetProperty(string propertyName)
        {
            return Properties.FirstOrDefault(it => it.Name.Equals(propertyName, System.StringComparison.OrdinalIgnoreCase));
        }

        [KIgnore]
        public override int GetHashCode()
        { 
            string unique = this.IsNested.ToString(); 
            if (_properties !=null)
            {
                foreach (var item in this.Properties)
                {
                    unique += item.GetHashCode().ToString(); 
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
