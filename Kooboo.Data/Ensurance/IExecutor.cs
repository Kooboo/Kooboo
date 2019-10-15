namespace Kooboo.Data.Ensurance
{
    public interface IExecutor<T> : IExecutor
    {
    }

    public interface IExecutor
    {
        bool Execute(string jsonModel);
    }
}