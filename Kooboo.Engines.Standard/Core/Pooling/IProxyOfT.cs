namespace SassAndCoffee.Core {
    using System;

    public interface IProxy<T> : IDisposable {
        Func<T, bool> OnDisposed { get; set; }
        T WrappedItem { get; set; }
    }
}
