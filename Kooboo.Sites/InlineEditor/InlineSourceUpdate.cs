//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.InlineEditor
{
    public class InlineSourceUpdate
    {
        public string KoobooId { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }

        public bool IsDelete { get; set; }
    }
}