//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Reflection;

 
namespace Kooboo.Data.Upgrade
{
    public static class UpgradeHelper
    {  
        public static string GetKoobooVersion(string koobooZipFile)
        {  
            using (var archive = ZipFile.Open(koobooZipFile, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count > 0)
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.IndexOf("kooboo.exe", StringComparison.OrdinalIgnoreCase) > -1)
                        { 
                            System.IO.MemoryStream mo = new MemoryStream();
                            entry.Open().CopyTo(mo); 
                            var bytes = mo.ToArray();
                            return GetVersion(bytes); 
                        }
                    }
                }
            }

            return null;
        }
        
        public static string GetVersion(byte[] DllBytes)
        {
            var assembely = Assembly.Load(DllBytes);

            var resource = assembely.GetManifestResourceNames().First(n => n.Equals("Kooboo.Data.dll",StringComparison.OrdinalIgnoreCase));

            if(resource != null)
            {
                using (var stream = assembely.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                        return null;

                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    try
                    {
                        var dataAssembly = Assembly.Load(bytes);
                        var version = dataAssembly.GetName().Version;
                        return version.ToString();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return null;
            
        }

    } 
}
