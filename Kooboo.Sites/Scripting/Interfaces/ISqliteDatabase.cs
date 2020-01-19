using Kooboo.Data.Attributes;

namespace KScript
{
    [KValueType(typeof(SqliteTable))]
    public interface ISqliteDatabase : IDatabase
    {
        int Execute(string sql);
        IDynamicTableObject[] Query(string sql);
    }
}