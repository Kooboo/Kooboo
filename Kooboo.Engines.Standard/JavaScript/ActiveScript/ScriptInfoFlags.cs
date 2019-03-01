//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript.ActiveScript {
    using System;

    [Flags]
    public enum ScriptInfoFlags : uint {
        /// <summary>
        /// Not a valid option.
        /// </summary>
        None = 0,

        /// <summary>
        /// Returns the IUnknown interface for this item.
        /// </summary>
        IUnknown = 1,

        /// <summary>
        /// Returns the ITypeInfo interface for this item.
        /// </summary>
        ITypeInfo = 2
    }
}
