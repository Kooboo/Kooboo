//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Sites.InlineEditor.Model
{
    public class ContentModel : IInlineModel
    {
        public ActionType Action
        {
            get; set;
        }

        public string NameOrId
        {
            get; set;
        }

        // used for copy content, the original content id or userkey.
        public string OrgNameOrId { get; set; }

        public string ObjectType
        {
            get; set;
        }

        public string FieldName { get; set; }

        public string Value
        {
            get; set;
        }

        public Guid FolderId { get; set; }

        public string EditorType
        {
            get
            {
                return "content";
            }
        }
    }
}