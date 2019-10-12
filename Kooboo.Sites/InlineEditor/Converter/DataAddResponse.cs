//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Contents.Models;
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public class DataAddResponse
    {
        public ContentFolder contentFolder { get; set; }

        public List<DateField> DateList = new List<DateField>();
    }

    public class DateField
    {
        public string Name { get; set; }

        public string Format { get; set; }
    }
}