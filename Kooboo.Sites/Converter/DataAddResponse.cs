//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Converter
{
   public class DataAddResponse
    {
        public ContentFolder contentFolder { get; set; }

        public List<DateField> DateList = new List<DateField>(); 
         
    }

    public class DateField
    {
        public string Name { get; set; }

        public string Format { get; set; }
    }
}
