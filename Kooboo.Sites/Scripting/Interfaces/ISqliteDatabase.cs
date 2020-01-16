namespace KScript
{
    public interface ISqliteDatabase : IDatabase
    {
        int Execute(string sql);
        object[] Query(string sql);
    }
}