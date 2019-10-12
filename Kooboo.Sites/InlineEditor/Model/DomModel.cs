//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.InlineEditor.Model
{
    public class DomModel : IInlineModel
    {
        public ActionType Action
        {
            get; set;
        }

        public string ObjectType { get; set; }

        public string KoobooId { get; set; }

        public string AttributeName { get; set; }

        public string Value { get; set; }

        public string NameOrId
        {
            get; set;
        }

        public string EditorType
        {
            get
            {
                return "dom";
            }
        }
    }
}