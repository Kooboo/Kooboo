//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{

    public class ChangePlan
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string ChangeInto { get; set; }
    }

    public class CmsCssRuleChanges
    {
        public CmsCssRuleChanges()
        {
            this.Declarations = new List<CmsCssDeclaration>();
        }

        public Guid CssRuleId { get; set; }
        public string selectorText { get; set; }

        public List<CmsCssDeclaration> Declarations;

        private string _declarationText;

        public string DeclarationText
        {
            get
            {
                if (string.IsNullOrEmpty(_declarationText))
                {
                    _declarationText = string.Empty;

                    foreach (var item in Declarations)
                    {
                        _declarationText += item.PropertyName + ": " + item.Value;
                        if (item.Important)
                        {
                            _declarationText += " !important";
                        }
                        _declarationText += ";\r\n";

                    }
                }
                return _declarationText;
            }
            set
            {
                _declarationText = value;
            }

        }

        public ChangeType ChangeType { get; set; }

        public Guid ParentCssRuleId { get; set; }

        private string _csstext;

        public string CssText
        {
            get
            {
                if (string.IsNullOrEmpty(_csstext))
                {
                    _csstext = selectorText;
                    if (!string.IsNullOrEmpty(DeclarationText))
                    {
                        _csstext += "\r\n{\r\n" + DeclarationText + "\r\n}";
                    }
                    else
                    {
                        if (selectorText.IndexOf("@import", StringComparison.OrdinalIgnoreCase)==-1)
                        {
                            _csstext += "\r\n{\r\n" + DeclarationText + "\r\n}";
                        }
                    }
                }

                return _csstext;
            }
            set
            {
                _csstext = value;
            }
        }

        public Guid ParentStyleId { get; set; }

        public Guid OwnerObjectId { get; set; }

        public byte OwnerConstType { get; set; }
    }

    public class InlineStyleChange
    {
        public InlineStyleChange()
        {
            this.PropertyValues = new Dictionary<string, string>(); 
        }

        public string KoobooId { get; set; }

     public   Dictionary<string, string> PropertyValues { get; set; }
 
    }

    public class changecompare : IComparer<ChangePlan>
    {

        public int Compare(ChangePlan x, ChangePlan y)
        {
            if (x.StartIndex > y.StartIndex)
            {
                return 1;
            }
            else if (x.StartIndex == y.StartIndex)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

}
