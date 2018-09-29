//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using System.Collections.Generic;
 
namespace Kooboo.Sites.Contents.Models
{
  public  class SystemFields  
    { 
        public static ContentProperty UserKey = new ContentProperty
        {
            Name = "UserKey",
            DataType = DataTypes.String,
            ControlType = ControlTypes.TextBox,
            Order = 100,
            Editable = false,
            IsSystemField = true
        }; 

        public static ContentProperty Online = new ContentProperty
        {
            Name = "Online",
            DataType = DataTypes.Bool,
            ControlType = ControlTypes.Boolean,
            Order = 101,
            Editable = true,
            IsSystemField = true 
        };

        public static ContentProperty Sequence = new ContentProperty
        {
            Name = "Sequence",
            DataType = DataTypes.Number,
            ControlType = ControlTypes.Number,
            Order = 93,
            Editable = false,
            IsSystemField = true
        };


        public static ContentProperty LastModified = new ContentProperty
        {
            Name = "LastModified",
            DataType = DataTypes.DateTime,
            ControlType = ControlTypes.DateTime,
            Order = 98,
            Editable = false,
            IsSystemField = true
        };

        private static List<string> _ReservedFields; 
        public static List<string> ReservedFields
        {
            get
            {
                if (_ReservedFields == null)
                {
                    _ReservedFields = new List<string>();
                    _ReservedFields.Add("folderid");
                    _ReservedFields.Add("id");
                    _ReservedFields.Add("parentid");
                    _ReservedFields.Add("contenttypeid");
                    _ReservedFields.Add("lastmodified");
                    _ReservedFields.Add("creationdate"); 
                }
                return _ReservedFields; 
            } 
 
        }

    }
}
