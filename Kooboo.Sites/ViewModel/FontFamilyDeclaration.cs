//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.ViewModel
{
    public class FontFamilyDeclaration
    { 
        public bool IsInline { get; set; }
        //for inline 
        public string KoobooId { get; set; }

        public string Selector { get; set; }

        public Guid CmsDeclarationId { get; set; }

        public Guid CmsCssRuleId { get; set; }

        public Guid ParentStyleId { get; set; }

        public string PropertyName { get; set; }

        public string Value { get; set; }

        private string _fontfamily; 
        public string FontFamily {
            get
            {
                if (string.IsNullOrEmpty(_fontfamily))
                {
                    if (_FontFamilyList != null)
                    {
                        return string.Join(",", _FontFamilyList.ToArray()); 
                    }
                }
                return _fontfamily; 
            }
            set { _fontfamily = value;  }
        }

        private List<string> _FontFamilyList; 

        public List<string> FontFamilyList {
            get
            { 
                if (_FontFamilyList == null)
                {
                    if (!string.IsNullOrWhiteSpace(_fontfamily))
                    {
                        _FontFamilyList = _fontfamily.Split(',').ToList(); 
                    }
                }
                return _FontFamilyList; 
            }
            set { _FontFamilyList = value;  }
        }

        public Guid OwnerObjectId { get; set; }

        public byte OwnerConstType { get; set; }

    }


   

}
