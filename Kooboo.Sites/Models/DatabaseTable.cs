
using System.Collections.Generic;

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

        public override int GetHashCode()
        {
            string unique = "";
            if (_columns != null)
            { 
                foreach (var item in _columns)
                {
                    unique += item.GetHashCode().ToString();
                }
            }
            return Lib.Security.Hash.ComputeInt(unique);
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
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public string ControlType { get; set; }
        public string Setting { get; set; }
        public bool IsSystem { get; set; }
        public int Length { get; set; }
        public override int GetHashCode()
        {
            string unique = this.Name + this.DataType + this.IsIncremental.ToString() + this.Seed.ToString() + this.Scale.ToString() + this.IsIndex.ToString();
            unique += this.IsPrimaryKey.ToString() + this.IsUnique.ToString() + this.ControlType.ToString() + this.Setting + this.IsSystem.ToString() + this.Length.ToString();
            return Lib.Security.Hash.ComputeInt(unique);
        }
    }


}