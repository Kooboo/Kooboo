//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    
    public class DatabaseColumnViewModel
    { 
        public string Name { get; set; }
 
        public string DataType
        {
            get;set;
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


    public class DatabaseItemEdit
    {
        public string Name { get; set; }

        public string DataType
        {
            get; set;
        }
         
        public object Value { get; set; }

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
    }

}
