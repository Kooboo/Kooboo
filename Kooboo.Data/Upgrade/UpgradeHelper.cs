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
                        if (entry.FullName.IndexOf("kooboo.data.dll", StringComparison.OrdinalIgnoreCase) > -1)
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
            var version = assembely.GetName().Version;
            return version.ToString();
        }

    } 
}
