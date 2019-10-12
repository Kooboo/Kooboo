//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public class DataField
    {
        public string Name { get; set; }
        public string ControlType { get; set; }
        public List<string> Values { get; set; }

        public bool IsRequired { get; set; }

        public List<DataField> Children { get; set; }
    }
}