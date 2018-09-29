//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.TAL.Binding
{
    public class ExpressionBindingInfo
    {
        public string ObjectType { get; set; }

        public object ObjectId { get; set; }

        public string Path { get; set; }

        public static ExpressionBindingInfo Content(Guid contentId, string fieldName)
        {
            return new ExpressionBindingInfo
            {
                ObjectType = "Content",
                ObjectId = contentId,
                Path = fieldName
            };
        }

        public static ExpressionBindingInfo Label(string name)
        {
            return new ExpressionBindingInfo
            {
                ObjectType = "Label",
                ObjectId = name
            };
        }
    }
}
