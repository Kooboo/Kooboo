using KScript;

namespace Kooboo.Sites.Scripting.Interfaces
{
    public interface IRelationalDatabase : IDatabase
    {
        string Source { get; }
        ISqlExecuter SqlExecuter { get; }
        IDynamicTableObject[] Query(string sql);
        IDynamicTableObject[] Query(string sql, object param = null);
        int Execute(string sql);
        int Execute(string sql, object param = null);
        object Procedure(string sql);
        object Procedure(string sql, object param = null);
    }
}
