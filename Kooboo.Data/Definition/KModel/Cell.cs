using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel
{
  public  class Cell
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EnumCellType Type { get; set; }  // Link, Text... 
         
        public string Style { get; set; }

        public string Class { get; set; }

        public EnumActionType Action { get; set; } 

        public string Url { get; set; } 
        //    cell: {
        //    type: "link",//cell type
        //    style: "",//cell style
        //    class: "",//cell class
        //    action: "newWindow",//cell action
        //    url: "/layout/detail?id={id}" //cell url
        //    } 
    }

    public enum EnumCellType
    {
        Text = 0, 
        Link = 1, 
    }

    public enum EnumActionType
    {
        NewWindow = 0,
        Redirect = 1,
        Ajax = 2
    } 
}
