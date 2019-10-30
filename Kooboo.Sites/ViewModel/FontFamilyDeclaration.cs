//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

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

        public string FontFamily
        {
            get
            {
                if (string.IsNullOrEmpty(_fontfamily))
                {
                    if (_fontFamilyList != null)
                    {
                        return string.Join(",", _fontFamilyList.ToArray());
                    }
                }
                return _fontfamily;
            }
            set { _fontfamily = value; }
        }

        private List<string> _fontFamilyList;

        public List<string> FontFamilyList
        {
            get
            {
                if (_fontFamilyList == null)
                {
                    if (!string.IsNullOrWhiteSpace(_fontfamily))
                    {
                        _fontFamilyList = _fontfamily.Split(',').ToList();
                    }
                }
                return _fontFamilyList;
            }
            set { _fontFamilyList = value; }
        }

        public Guid OwnerObjectId { get; set; }

        public byte OwnerConstType { get; set; }
    }
}