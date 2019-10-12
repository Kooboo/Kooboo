//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.Constraints
{
    public class ConstraintResponse
    {
        public string ContraintName { get; set; }
        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public string AffectedPart { get; set; }

        public string Message { get; set; }
    }
}