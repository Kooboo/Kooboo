//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;

    public interface IInstanceProvider<T>
        where T : IDisposable {
        T GetInstance();
    }
}
