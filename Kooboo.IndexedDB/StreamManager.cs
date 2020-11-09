//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace Kooboo.IndexedDB
{
  public  class StreamManager
    {
        const FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;

        public static FileStream GetFileStream(string FullFileName)
        {
            return File.Open(FullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

           // return new FileStream(FullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.WriteThrough); 
        }

        public static FileStream OpenReadStream(string FullFileName)
        { 
            return File.Open(FullFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); 
        }
        
        //public static FileStream GetFileStream(string FullFileName)
        //{
        //    return new FileStream(FullFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete, 4096*2, 
        //    FileFlagNoBuffering | FileOptions.WriteThrough);

        //}
       
    }
}
