//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Kooboo.Sites.Models
{ 
    public class CmsCssRule : SiteObject
    {
        ///How to identify a css rule. 
        // When this is a css rule under a style sheet, identified by the style sheet id and itemindex. if this is also under a like media rule, should also check into the media rule. 
        /// <summary>
        /// Css Rule of StyleSheet.  This is the CMS CSS Rule, Not the DOM Css Rule.
        /// </summary>
        /// <param name="selectorText"></param>
        /// <param name="ItemIndex"></param>
        public CmsCssRule()
        {
            this.ConstType = ConstObjectType.CssRule;
            this.ItemIndex = 0;
        }

        private Guid _id;
        public override Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {

                    string unique = this.ConstType.ToString() + this.ParentStyleId +  this.SelectorText + this.DuplicateIndex.ToString();

                    if (this.ParentCssRuleId != default(Guid))
                    {
                        unique += this.ParentCssRuleId.ToString(); 
                    }

                    if (this.IsInline)
                    {
                        unique += this.OwnerObjectId.ToString();
                        unique += this.KoobooId;
                    }
                    _id = Kooboo.Data.IDGenerator.GetId(unique);
                }
                return _id;
            }
        }

        /// <summary>
        /// The item position index in the parent cssrule or stylye.
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public int ItemIndex { get; set; }

        
        [Kooboo.Attributes.SummaryIgnore]
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public int TempCssRuleIndex { get; set; }

        /// <summary>
        /// If there are duplicate css rules, it should be numbered like 0,1,2,3,4...in order to identify them.
        /// </summary>        [Kooboo.Attributes.SummaryIgnore]
        public int DuplicateIndex { get; set; }


        /// <summary>
        /// The style sheet that this rule belongs to. 
        /// </summary>
       [Kooboo.Attributes.SummaryIgnore]
        public Guid ParentStyleId { get; set; }

        /// <summary>
        /// The parent rule this rule belongs to. for example, a parent media rule. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public Guid ParentCssRuleId { get; set; }

        /// <summary>
        ///  the Css rule text. 
        /// </summary>
        public string CssText { get; set; }

        [Kooboo.Attributes.SummaryIgnore]
        public int selectorPositionIndex { get; set; }

        /// <summary>
        /// The selector text part, for At-rule, it is @import, @font-face or @media + media condition. 
        /// </summary>
        public string SelectorText
        {
            get
            {
                if (this.selectorPositionIndex <= 0)
                {
                    /// try to make again...
                    if (!string.IsNullOrEmpty(this.CssText))
                    {
                        int bracketindex = this.CssText.IndexOf("{");

                        if (bracketindex > 0)
                        {
                            this.selectorPositionIndex = bracketindex;
                        }
                    }

                }

                if (this.selectorPositionIndex <= 0)
                {
                    return string.Empty;
                }
                else
                {

                    if (this.ruleType == RuleType.ImportRule || this.ruleType == RuleType.FontFaceRule)
                    {
                        return string.Empty;
                    }
                    else if (this.ruleType == RuleType.MediaRule)
                    {
                        var startPosition = this.CssText.ToLower().IndexOf("@media")+6;
                        return this.CssText.Substring(startPosition, this.selectorPositionIndex - startPosition).Trim();
                    }
                    else
                    {
                        return this.CssText.Substring(0, this.selectorPositionIndex).Trim();
                    }
                }
            }
        }

 
        ///  The rule text body of this rule, for stylerule, this is the style decleration block text. 
        public string RuleText
        {
            get
            {
                if (this.selectorPositionIndex <= 0)
                {
                    return CssText;
                }

                else
                {
                    return this.CssText.Substring(this.selectorPositionIndex + 1, this.CssText.Length - this.selectorPositionIndex - 2);
                }
            }
        }

        [Kooboo.Attributes.SummaryIgnore]
        public bool IsInline
        {
            get;
            set;
        }

        /// <summary>
        /// In the case of inline style, the Id of the container object. like page, view or layout. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public Guid OwnerObjectId
        {
            get;set;
        }

        /// <summary>
        /// The constObjectType of ObjectId. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public byte OwnerObjectConstType { get; set; }

        /// <summary>
        /// The kooboo tag id when it is an inline style rule. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public string KoobooId { get; set; }

        /// <summary>
        /// The open tag content of Kooboo Id, this field is only used for quick access for display information only. 
        /// </summary>
        public string KoobooOpenTag { get; set; }

        private string _displayName;

        /// <summary>
        /// The display name of this Rule when it is an inline style rule. 
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    return this.SelectorText;
                }
                else
                {
                    return _displayName;
                }
            }
            set
            {
                _displayName = value;
            }
        }

        [Kooboo.Attributes.SummaryIgnore]
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleType ruleType { get; set; }
         
        public override int GetHashCode()
        {
            string unique = this.CssText + this.SelectorText + this.RuleText; 

            if (this.IsInline)
            {
                unique += this.KoobooOpenTag; 
            }

            unique += this.OwnerObjectId.ToString(); 

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

        private List<string> _properties; 
        public List<string> Properties {
            get {
                if (_properties == null)
                {
                    _properties = new List<string>();
                }
                return _properties; 
            }
            set { _properties = value;  }
        }
        
    }

    public enum RuleType
    {
        StyleRule = 0,
        MediaRule = 1,
        ImportRule = 2,
        FontFaceRule = 3
    }




}
