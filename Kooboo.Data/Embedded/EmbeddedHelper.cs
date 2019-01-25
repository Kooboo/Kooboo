//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Embedded
{

    public static class EmbeddedHelper
    {

        public static byte[] GetBytes(string filename, Type assemblyType)
        {
            filename = filename.ToLower();

            var assembly = assemblyType.Assembly;

            var allnames = assembly.GetManifestResourceNames();

            string fullname = null;

            foreach (var item in allnames)
            {
                if (item != null && item.ToLower().Contains(filename))
                {
                    fullname = item;
                }
            }

            if (!string.IsNullOrEmpty(fullname))
            {
                var stream = assembly.GetManifestResourceStream(fullname);

                System.IO.MemoryStream mo = new System.IO.MemoryStream();
                stream.CopyTo(mo);

                return mo.ToArray();
            }
            return null;
        }


        public static StreamReader GetStreamReader(string filename, Type assemblyType)
        {
            filename = filename.ToLower();

            var assembly = assemblyType.Assembly;

            var allnames = assembly.GetManifestResourceNames();

            string fullname = null;

            foreach (var item in allnames)
            {
                if (item != null && item.ToLower().Contains(filename))
                {
                    fullname = item;
                }
            }

            if (!string.IsNullOrEmpty(fullname))
            {
                var stream = assembly.GetManifestResourceStream(fullname);

                return new System.IO.StreamReader(stream);
            }
            return null;
        }


    }



}
