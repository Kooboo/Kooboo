using Kooboo.Data.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Models
{
   public  class TableRelation : CoreObject
    {
        public TableRelation()
        {
            this.ConstType = ConstObjectType.TableRelation; 
        }

        public string TableA { get; set; }

        public string FieldA { get; set; }
         
        public string TableB { get; set; }

        public string FieldB { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumTableRelation Relation { get; set; }

        public override int GetHashCode()
        {
            string unique = this.TableA + this.TableB + this.FieldA + this.FieldB + this.Relation.ToString();
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }

    }

    public enum EnumTableRelation
    {
        OneOne = 1,
        OneMany = 2,
        ManyMany = 3,
        ManyOne = 4 
    }
}
