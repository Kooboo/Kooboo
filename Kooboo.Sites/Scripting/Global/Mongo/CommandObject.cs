namespace KScript
{
    /// <summary>
    /// contains aggregate find insert update delete
    /// </summary>
    public class CommandObject
    {
        public object aggregate { get; set; }
        public object[] pipeline { get; set; }
        public bool explain { get; set; }
        public bool allowDiskUse { get; set; }
        public object cursor { get; set; }
        public int maxTimeMS { get; set; }
        public bool bypassDocumentValidation { get; set; }
        public object readConcern { get; set; }
        public object collation { get; set; }
        public object hint { get; set; }
        public string comment { get; set; }
        public object writeConcern { get; set; }
        public string delete { get; set; }
        public object[] deletes { get; set; }
        public bool ordered { get; set; }
        public string find { get; set; }
        public object filter { get; set; }
        public object sort { get; set; }
        public object projection { get; set; }
        public int skip { get; set; }
        public int limit { get; set; }
        public int batchSize { get; set; }
        public bool singleBatch { get; set; }
        public object max { get; set; }
        public object min { get; set; }
        public bool returnKey { get; set; }
        public bool showRecordId { get; set; }
        public bool tailable { get; set; }
        public bool oplogReplay { get; set; }
        public bool noCursorTimeout { get; set; }
        public bool awaitData { get; set; }
        public bool allowPartialResults { get; set; }
        public string insert { get; set; }
        public object[] documents { get; set; }
        public string update { get; set; }
        public object[] updates { get; set; }
    }
}