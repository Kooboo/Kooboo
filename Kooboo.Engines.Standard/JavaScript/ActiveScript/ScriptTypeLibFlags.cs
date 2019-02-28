//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript.ActiveScript {
    using System;

    [Flags]
    public enum ScriptTypeLibFlags : uint {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// The type library describes an ActiveX control used by the host.
        /// </summary>
        IsControl = 0x00000010,

        /// <summary>
        /// Not documented.
        /// </summary>
        IsPersistent = 0x00000040,
    }
}
