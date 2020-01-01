//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using KScript;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KScript
{
    public class UploadFile
    {
        private RenderContext context { get; set; }


        public UploadFile(RenderContext context)
        {
            this.context = context;
        }

        public string ContentType { get; set; }

        [Description("Uploaded file name")]
        public string FileName { get; set; }
         
        public byte[] Bytes { get; set; }

        [Description(@"Save uploaded file into disk
       k.request.files.forEach(function(item)
        { 
         k.response.write(item.fileName); 
         item.save(item.fileName);  
        })  
      } ")]
        public FileInfo Save(string filename)
        {
            FileIO io = new FileIO(this.context);
            return  io.writeBinary(filename, this.Bytes);     
        }

    }
}
