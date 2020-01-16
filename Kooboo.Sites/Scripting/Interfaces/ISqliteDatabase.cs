namespace KScript
{
    public interface ISqliteDatabase : IDatabase
    {
        int Execute(string sql);
        IDynamicTableObject[] Query(string sql);
    }
}