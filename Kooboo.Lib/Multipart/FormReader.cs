//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Kooboo.Lib.NETMultiplePart
{
    public static class FormReader
    {
        public static List<File> ReadFile(byte[] input)
        {
            List<File> result = new List<File>();
            string boundary = Boundary.GetBoundary(input);
            if (!string.IsNullOrEmpty(boundary))
            {
                var reader = new Microsoft.AspNet.WebUtilities.MultipartReader(boundary, new MemoryStream(input));

                var next = reader.ReadNextSectionAsync();
                var sectionResult = next.Result;

                while (sectionResult != null)
                {
                    File file = new File();
                    MemoryStream memory = new MemoryStream();
                    sectionResult.Body.CopyTo(memory);
                    file.Bytes = memory.ToArray();
                    file.ContentType = sectionResult.ContentType;

                    var attributes = AttributeReader.GetAttributes(sectionResult.ContentDisposition);

                    if (attributes.ContainsKey("filename"))
                    {
                        file.FileName = attributes["filename"];
                    }
                    else if (attributes.ContainsKey("name"))
                    {
                        file.FileName = attributes["name"];
                    }
                    result.Add(file);
                    next = reader.ReadNextSectionAsync();
                    if (next == null)
                    {
                        break;
                    }
                    sectionResult = next.Result;
                }


            }
            return result;
        }

        public static FormResult ReadForm(byte[] input)
        {
            FormResult result = new FormResult();

            string boundary = Boundary.GetBoundary(input);
            if (!string.IsNullOrEmpty(boundary))
            {
                var reader = new Microsoft.AspNet.WebUtilities.MultipartReader(boundary, new MemoryStream(input));

                var next = reader.ReadNextSectionAsync();
                var sectionResult = next.Result;

                while (sectionResult != null)
                {
                    var attributes = AttributeReader.GetAttributes(sectionResult.ContentDisposition);

                    MemoryStream memory = new MemoryStream();
                    sectionResult.Body.CopyTo(memory);

                    if (attributes.ContainsKey("filename"))
                    {
                        File file = new File();
                        file.Bytes = memory.ToArray();
                        file.ContentType = sectionResult.ContentType;

                        if (attributes.ContainsKey("filename"))
                        {
                            file.FileName = attributes["filename"];
                        }
                        else if (attributes.ContainsKey("name"))
                        {
                            file.FileName = attributes["name"];
                        }
                        result.Files.Add(file);
                    }
                    else if (attributes.ContainsKey("name"))
                    {
                        string parakey = attributes["name"];
                        if (!string.IsNullOrEmpty(parakey))
                        {
                            var bytes = memory.ToArray();
                            var encoding = Helper.EncodingDetector.GetEncoding(ref bytes);
                            string paravalue = encoding.GetString(bytes);
                            result.FormData.Add(parakey, paravalue);
                        }
                    }

                    next = reader.ReadNextSectionAsync();
                    if (next == null)
                    {
                        break;
                    }
                    sectionResult = next.Result;
                }


            }
            return result;
        }
    }

    public class File
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }

        private byte[] _bytes;
        public byte[] Bytes
        {
            get
            {
                if (_bytes == null || _bytes.Length == 0)
                {
                    _bytes = Stream.ToArray();
                }
                return _bytes;
            }
            set { _bytes = value; }
        }

        private MemoryStream _stream;
        [JsonIgnore]
        public MemoryStream Stream
        {
            get
            {
                if (_stream == null)
                {
                    _stream = new MemoryStream();
                    CopyTo(_stream);
                }
                return _stream;
            }
            set
            {
                _stream = value;
            }
        }

        [JsonIgnore]
        public Action<Stream> CopyTo { get; set; }
    }

    public class FormResult
    {
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public List<File> Files { get; set; } = new List<File>();
    }
}
