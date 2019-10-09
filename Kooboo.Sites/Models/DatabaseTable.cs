using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Models
{
    public class DatabaseTable : CoreObject
    {

        public DatabaseTable()
        {
            this.ConstType = ConstObjectType.DatabaseTable;
        }

        private List<DbTableColumn> _columns;

        public List<DbTableColumn> Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new List<DbTableColumn>();
                }

                return _columns; 
            }
            set
            {
                _columns = value;
            }
        }

    }


    public class DbTableColumn
    {
        public string Name { get; set; }

        public string DataType
        {
            get; set;
        }

        public bool IsIncremental { get; set; }

        public long Seed { get; set; }

        public int Scale { get; set; }

        public bool IsIndex { get; set; }

        // when this is set to an value non defined as "_id", it means the _id field will be hash from this value. 
        // Then there will be at least two index instantly... Fine... 
        public bool IsPrimaryKey { get; set; }

        public bool IsUnique { get; set; }

        public string ControlType { get; set; }

        public string Setting { get; set; }

        public bool IsSystem { get; set; }

        public int Length { get; set; }
    }


}
