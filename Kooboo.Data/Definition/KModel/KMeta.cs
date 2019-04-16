using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel
{
  public   class KMeta
    {
        public List<Column> Columns { get; set; } = new List<Column>(); 

    }

  //  var meta = {
  //columns: [
  //  {
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
  //  },
  //]


}
