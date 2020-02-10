namespace KScript
{
    public class CommandResult
    {
        public class Cursor
        {
            public object[] firstBatch { get; set; }
            public int id { get; set; }
            public string ns { get; set; }
        }
        public Cursor cursor { get; set; }
        public int ok { get; set; }
    }
}