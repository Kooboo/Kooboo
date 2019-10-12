//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public class ConvertSourceUpdate
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string NewValue { get; set; }
        public Guid PageId { get; set; }
    }
}