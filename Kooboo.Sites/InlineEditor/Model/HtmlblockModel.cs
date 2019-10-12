//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.InlineEditor.Model

{
    public class HtmlblockModel : IInlineModel
    {
        public ActionType Action
        {
            get; set;
        }

        public string EditorType
        {
            get
            {
                return "htmlblock";
            }
        }

        public string NameOrId
        {
            get; set;
        }

        public string ObjectType
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }
    }
}