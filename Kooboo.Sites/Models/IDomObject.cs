//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// Dom object contains one or more Dom property that can be used to calculate relations, etc... 
    /// </summary>
    public interface IDomObject :  ITextObject
    {
        [Newtonsoft.Json.JsonIgnore] 
        Document Dom { get; }
    }
}