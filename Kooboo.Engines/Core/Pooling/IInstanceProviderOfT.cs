namespace SassAndCoffee.Core {
    using System;

    public interface IInstanceProvider<T>
        where T : IDisposable {
        T GetInstance();
    }
}
