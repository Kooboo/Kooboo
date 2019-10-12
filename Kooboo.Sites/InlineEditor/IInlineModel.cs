//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.InlineEditor
{
    public interface IInlineModel
    {
        string EditorType { get; }

        string ObjectType { get; set; }

        ActionType Action { get; set; }

        string NameOrId { get; set; }

        string Value { get; set; }
    }
}