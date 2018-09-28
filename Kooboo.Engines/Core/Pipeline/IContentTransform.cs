namespace SassAndCoffee.Core {
    using System;

    public interface IContentTransform : IDisposable {
        void PreExecute(ContentTransformState state);
        void Execute(ContentTransformState state);
    }
}
