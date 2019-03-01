//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;

    public interface IProxy<T> : IDisposable {
        Func<T, bool> OnDisposed { get; set; }
        T WrappedItem { get; set; }
    }
}
