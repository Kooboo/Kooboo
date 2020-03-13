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
        readonly ZipArchiveEntry _zipArchiveEntry;
        ZipOption _zipOption;
        byte[] _cache;

        public ZipFile(ZipArchiveEntry zipArchiveEntry, string path, string zipPath, ZipOption zipOption = null) : base(path, "zip")
        {
            _zipArchiveEntry = zipArchiveEntry;
            _zipOption = zipOption ?? new ZipOption();
            ZipPath = zipPath;
        }


        public string ZipPath { get; set; }

        public override Stream Open()
        {
            return _zipArchiveEntry.Open();
        }

        public override byte[] ReadAllBytes()
        {
            if (_cache != null) return _cache;
            using (var stream = _zipArchiveEntry.Open())
            {
                var bytes = new byte[_zipArchiveEntry.Length];
                if (_zipArchiveEntry.Length > Int32.MaxValue) throw new IOException();
                stream.Read(bytes, 0, (int)_zipArchiveEntry.Length);
                if (_zipOption.Cache) _cache = bytes;
                return bytes;
            }
        }

        public override string ReadAllText()
        {
            return ReadAllText(_zipOption.Encoding ?? Encoding.Default);
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
