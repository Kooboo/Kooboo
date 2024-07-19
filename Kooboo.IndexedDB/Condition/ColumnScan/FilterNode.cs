using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Condition.ColumnScan
{
    public class FilterNode : Node
    {
        /// <summary>
        /// The field or property name of this column.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// the relative start position within the entire column data bytes. 
        /// </summary>
        public int RelativeStartPosition { get; set; }

        /// <summary>
        /// The byte length of this column in the column data. 
        /// </summary>
        public int Length { get; set; }

        public ColumnEvaluator Evaluator;

        protected override NodeType NodeType => NodeType.Filter;
    }
}