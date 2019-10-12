//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Sites.InlineEditor.Model
{
    public class ImageModel : IInlineModel
    {
        public string NameOrId
        {
            get; set;
        }

        public Guid ImageId { get; set; }

        public string Value
        {
            get; set;
        }

        public string EditorType
        {
            get
            {
                return "image";
            }
        }

        // styel or dom..
        public string ObjectType
        {
            get; set;
        }

        // only for the style update.....
        public string PropertyName { get; set; }

        // for dom object image.
        public string KoobooId { get; set; }

        public ActionType Action
        {
            get; set;
        }
    }
}