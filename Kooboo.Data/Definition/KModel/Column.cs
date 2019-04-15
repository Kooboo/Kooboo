using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel
{
 public   class Column
    { 
        public string Name { get; set; }

        public Header Header { get; set; }
        public Cell Cell { get; set; }

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
