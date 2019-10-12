//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Sites.InlineEditor
{
    public class CmsRuleChange
    {
        public Guid CmsRuleId { get; set; }

        public string PropertyName { get; set; }

        public string Value { get; set; }

        public bool Important { get; set; }
    }
}