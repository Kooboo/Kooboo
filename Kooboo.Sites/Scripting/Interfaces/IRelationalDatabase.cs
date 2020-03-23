using KScript;

namespace Kooboo.Sites.Scripting.Interfaces
{
    public interface IRelationalDatabase : IDatabase
    {
        string Source { get; }
        ISqlExecuter SqlExecuter { get; }
        IDynamicTableObject[] Query(string sql);
        int Execute(string sql, object param = null);
    }
}
