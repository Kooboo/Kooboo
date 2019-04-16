using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel
{
 public   class Column
    { 
        public Column()
        {
            this.Header = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.Cell = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 
        }

        public string Name { get; set; }

        public Dictionary<string, string> Header { get; set; }

        public Dictionary<string,string> Cell { get; set; }  
    }
     

    //     name: "online",//column name
    //    header: { //table header
    //      displayName: "在线情况",//header displayName
    //      class: "",//header class
    //      style: ""//header style
    //    },
    //    cell: {
    //      type: "link",//cell type
    //      style: "",//cell style
    //      class: "",//cell class
    //      action: "newWindow",//cell action
    //      url: "/layout/detail?id={id}" //cell url
    //    }


}
