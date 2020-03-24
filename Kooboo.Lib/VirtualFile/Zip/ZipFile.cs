using ICSharpCode.SharpZipLib.Core;
using Kooboo.Lib.VirtualFile.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace VirtualFile.Zip
{
    public class ZipFile : VirtualFile
    {
        readonly ICSharpCode.SharpZipLib.Zip.ZipEntry _zipArchiveEntry;
        readonly ICSharpCode.SharpZipLib.Zip.ZipFile _zipArchive;
        ZipOption _zipOption;
        byte[] _cache;

        public ZipFile(
            ICSharpCode.SharpZipLib.Zip.ZipEntry zipArchiveEntry,
            ICSharpCode.SharpZipLib.Zip.ZipFile zipArchive,
            string path, string zipPath,
            ZipOption zipOption = null) : base(path, "zip")
        {
            _zipArchiveEntry = zipArchiveEntry;
            _zipArchive = zipArchive;
            _zipOption = zipOption ?? new ZipOption();
            ZipPath = zipPath;
        }


        public string ZipPath { get; set; }

        public override Stream Open()
        {
            return _zipArchive.GetInputStream(_zipArchiveEntry);
        }

        public override byte[] ReadAllBytes()
        {
            if (_cache != null) return _cache;
            var temp = new byte[_zipArchiveEntry.Size];

            using (var stream = _zipArchive.GetInputStream(_zipArchiveEntry))
            {
                StreamUtils.ReadFully(stream, temp);
            }

            if (_zipOption.Cache) _cache = temp;
            return temp;
        }

        public override string ReadAllText(Encoding encoding)
        {
            var bytes = _cache;

            if (bytes == null)
            {
                bytes = ReadAllBytes();
                if (_zipOption.Cache) _cache = bytes;
            }

            return encoding.GetString(bytes);
        }
    }
}
