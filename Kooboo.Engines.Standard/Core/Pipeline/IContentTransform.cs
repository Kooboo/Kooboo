//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;

    public interface IContentTransform : IDisposable {
        void PreExecute(ContentTransformState state);
        void Execute(ContentTransformState state);
    }
}
