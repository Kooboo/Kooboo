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
            if (!System.IO.Path.IsPathRooted(koobooZipFile))
            {
                koobooZipFile = System.IO.Path.GetFullPath(koobooZipFile); 
            }

            if (System.IO.File.Exists(koobooZipFile))
            {
                var allbytes = System.IO.File.ReadAllBytes(koobooZipFile);

                return GetKoobooVersion(allbytes);
            }
            return null;
        }

        public static string GetKoobooVersion(byte[] allbytes)
        {                              
            var koobooextbytes = ExtractFileFromZip(allbytes, "kooboo.exe");   
            return GetVersionFromKoobooExe(koobooextbytes);
        }

        public static string GetLinuxKoobooVersion(byte[] allbytes)
        {
            var dataBytes = ExtractFileFromZip(allbytes, "Kooboo.Data.dll");
            if (dataBytes != null)
            {
                var version = GetDllVersion(dataBytes);
                return version != null ? version.ToString() : null;
            }
            return null;
        }
                   
        //TODO: should move to Kooboo.Lib
        public static byte[] ExtractFileFromZip(byte[] ZipBytes, string ContainsName)
        {       
            System.IO.MemoryStream mo = new MemoryStream(ZipBytes);
                                    
            using (var archive = new ZipArchive(mo, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count > 0)
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.IndexOf(ContainsName, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            System.IO.MemoryStream part = new MemoryStream();
                            entry.Open().CopyTo(part);
                            return part.ToArray();   
                        }
                    }
                }  
            }

            return null; 
        }
                                                            
        public static string GetVersionFromKoobooExe(byte[] DllBytes)
        {
            // use kooboo.data.dll to define the version. 
            var kooboodatabytes = GetManifestResourceFile(DllBytes, "Kooboo.Data.dll"); 
            if (kooboodatabytes !=null)
            {
                var version = GetDllVersion(kooboodatabytes);
                return version != null ? version.ToString() : null; 
            }
            return null; 
        }

        // TODO: can move to kooboo.lib.
        public static byte[] GetManifestResourceFile(byte[] ContainterBinary, string FileName)
        {
            var assembely = Assembly.Load(ContainterBinary); 
            var resource = assembely.GetManifestResourceNames().First(n => n.Equals(FileName, StringComparison.OrdinalIgnoreCase));

            if (resource != null)
            {
                using (var stream = assembely.GetManifestResourceStream(resource))
                {
                    if (stream !=null)
                    {    
                        var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        return memoryStream.ToArray(); 
                    }  
                }
            }
            return null;
        }

        public static Version GetDllVersion(byte[] dllbytes)
        {
            var dataAssembly = Assembly.Load(dllbytes);
            if (dataAssembly !=null)
            {
                return dataAssembly.GetName().Version;
            }

            return null;    
        }
    } 
}
