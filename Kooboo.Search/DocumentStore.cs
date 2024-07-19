//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Search
{
    //TODO: allow some 10% extra space for update doc in the same disk position. 
    public class DocumentStore
    {
        private object _locker = new object();

        private Encoding Encoding { get; set; } = Encoding.UTF8;

        internal string DocumentFolder { get; set; }

        internal string IndexFile
        {
            get
            {
                if (!string.IsNullOrEmpty(DocumentFolder))
                {
                    return System.IO.Path.Combine(DocumentFolder, "doc.index");
                }
                return null;
            }
        }

        internal string DataFile
        {
            get
            {
                if (!string.IsNullOrEmpty(DocumentFolder))
                {
                    return System.IO.Path.Combine(DocumentFolder, "doc.data");
                }
                return null;
            }
        }

        // id and position. 
        public Dictionary<int, DocInfo> Docs { get; set; } = new Dictionary<int, DocInfo>();

        public DocumentStore(string Folder)
        {
            this.DocumentFolder = Folder;

            if (string.IsNullOrWhiteSpace(Folder))
            {
                throw new Exception("document folder not provider");
            }
            Lib.Helper.IOHelper.EnsureDirectoryExists(Folder);
            this.LoadIndex();
        }

        public void LoadIndex(string fileName = null)
        {
            if (fileName == null)
            {
                fileName = this.IndexFile;
            }

            if (File.Exists(fileName))
            {
                var allBytes = Lib.Helper.IOHelper.ReadAllBytes(fileName);
                int len = allBytes.Length;

                int maxindex = (int)(len / 24);
                if (maxindex * 24 > len)
                {
                    maxindex = maxindex - 1;
                }
                for (int i = 0; i < maxindex; i++)
                {
                    DocInfo info = new DocInfo();

                    byte[] infobytes = new byte[24];
                    Buffer.BlockCopy(allBytes, i * 24, infobytes, 0, 24);
                    info.FromBytes(infobytes);
                    if (info.Position > 0)
                    {
                        this.Docs[i] = info;
                    }
                }
            }
        }

        public int Add(string body, string meta, bool storebody = true)
        {
            Guid metahash = Lib.Security.Hash.ComputeGuidIgnoreCase(meta);
            if (storebody)
            {
                var currentposition = this.AddDoc(body, meta);
                return this.AddIndex(currentposition, metahash);
            }
            else
            {
                var currentposition = this.AddDoc(null, meta);
                return this.AddIndex(currentposition, metahash);
            }
        }

        internal long AddDoc(string body, string meta)
        {
            byte[] bodyBytes = null;
            int bodyLen = 0;
            int bodyBufferLen = 0;
            byte[] metaBytes = null;
            int metaLen = 0;
            if (!string.IsNullOrEmpty(body))
            {
                bodyBytes = this.Encoding.GetBytes(body);
                if (bodyBytes != null)
                {
                    bodyLen = bodyBytes.Length;
                }

                int extra = (int)bodyLen / 10;
                if (extra < 50)
                {
                    extra = 50;
                }
                bodyBufferLen = bodyLen + extra;
            }

            if (!string.IsNullOrEmpty(meta))
            {
                metaBytes = this.Encoding.GetBytes(meta);
                if (metaBytes != null)
                {
                    metaLen = metaBytes.Length;
                }
            }

            int totalLen = bodyBufferLen + metaLen;

            byte[] diskbytes = new byte[totalLen + 14];

            diskbytes[0] = 10;
            diskbytes[1] = 13;
            Buffer.BlockCopy(BitConverter.GetBytes(metaLen), 0, diskbytes, 2, 4);

            Buffer.BlockCopy(BitConverter.GetBytes(bodyLen), 0, diskbytes, 6, 4);

            Buffer.BlockCopy(BitConverter.GetBytes(bodyBufferLen), 0, diskbytes, 10, 4);


            if (metaLen > 0)
            {
                Buffer.BlockCopy(metaBytes, 0, diskbytes, 14, metaLen);
            }

            if (bodyLen > 0)
            {
                Buffer.BlockCopy(bodyBytes, 0, diskbytes, 14 + metaLen, bodyLen);
            }
            long currentposition = this.DataStream.Length;

            lock (_locker)
            {
                this.DataStream.Position = currentposition;
                this.DataStream.Write(diskbytes, 0, totalLen + 14);
            }

            return currentposition;
        }

        public IndexDocument LoadDoc(long Position)
        {
            lock (_locker)
            {
                byte[] header = new byte[14];
                this.DataStream.Position = Position;

                this.DataStream.Read(header, 0, 14);

                if (header[0] != 10 || header[1] != 13)
                {
                    return null;
                }

                int metalen = BitConverter.ToInt32(header, 2);
                int bodylen = BitConverter.ToInt32(header, 6);
                int bodybufferlen = BitConverter.ToInt32(header, 10);

                int totallen = metalen + bodylen;
                byte[] content = new byte[totallen];

                this.DataStream.Position = Position + 14;
                this.DataStream.Read(content, 0, totallen);

                IndexDocument doc = new IndexDocument() { MetaLen = metalen, BodyBufferLen = bodybufferlen, BodyLen = bodylen };

                if (metalen > 0)
                {
                    doc.Meta = this.Encoding.GetString(content, 0, metalen);
                }

                if (bodylen > 0)
                {
                    doc.Body = this.Encoding.GetString(content, metalen, bodylen);
                }

                return doc;
            }

        }

        private string LoadMeta(long Position)
        {
            lock (_locker)
            {
                byte[] header = new byte[10];
                this.DataStream.Position = Position;

                this.DataStream.Read(header, 0, 10);

                if (header[0] != 10 || header[1] != 13)
                {
                    return null;
                }

                int metalen = BitConverter.ToInt32(header, 2);
                if (metalen > 0)
                {
                    byte[] content = new byte[metalen];

                    this.DataStream.Position = Position + 14;
                    this.DataStream.Read(content, 0, metalen);

                    return this.Encoding.GetString(content, 0, metalen);
                }

                return null;
            }

        }

        public void Delete(int id)
        {
            if (this.Docs.ContainsKey(id))
            {
                var info = this.Docs[id];
                lock (_locker)
                {
                    this.DeleteIndex(id);
                    this.Docs.Remove(id);
                    byte[] removeheader = new byte[14];

                    this.DataStream.Position = info.Position;
                    this.DataStream.Write(removeheader, 0, 14);
                }
            }
        }

        public void Update(int id, string body)
        {
            if (this.Docs.ContainsKey(id))
            {
                var info = this.Docs[id];

                var currentdoc = LoadDoc(info.Position);

                if (currentdoc != null)
                {
                    var newposition = updatebody(info.Position, currentdoc, body, currentdoc.Meta);

                    UpdateIndex(id, newposition, currentdoc.Meta);
                }
            }
        }

        private long updatebody(long position, IndexDocument doc, string newbody, string meta)
        {
            int bodylen = 0;
            var bodybytes = this.Encoding.GetBytes(newbody);
            if (bodybytes != null)
            {
                bodylen = bodybytes.Length;
            }

            if (bodylen < doc.BodyBufferLen)
            {
                this.DataStream.Position = position + 6;
                this.DataStream.Write(BitConverter.GetBytes(bodylen), 0, 4);

                //write body bytes. 
                this.DataStream.Position = position + 14 + doc.MetaLen;
                this.DataStream.Write(bodybytes, 0, bodylen);
                return position;
            }
            else
            {
                return AddDoc(newbody, meta);
            }

        }

        public IndexDocument Get(int id)
        {
            if (this.Docs.ContainsKey(id))
            {
                var info = this.Docs[id];
                return LoadDoc(info.Position);
            }
            return null;
        }

        public string GetMeta(int id)
        {
            if (this.Docs.ContainsKey(id))
            {
                var info = this.Docs[id];
                return LoadMeta(info.Position);
            }
            return null;
        }

        public void DeleteIndex(int id)
        {
            var position = id * 24;

            var total = this.IndexStream.Length;

            if (total >= position + 24)
            {
                lock (_locker)
                {
                    if (this.Docs.ContainsKey(id))
                    {
                        this.IndexStream.Position = position;
                        this.IndexStream.Write(new byte[24], 0, 24);
                        this.Docs.Remove(id);
                    }

                }
            }
        }

        public void UpdateIndex(int id, long newposition, string meta)
        {
            var position = id * 24;
            Guid metahash = Lib.Security.Hash.ComputeGuidIgnoreCase(meta);
            var total = this.IndexStream.Length;
            if (total >= position + 24)
            {
                lock (_locker)
                {
                    if (this.Docs.ContainsKey(id))
                    {
                        this.IndexStream.Position = position;
                        DocInfo newinfo = new DocInfo() { Position = newposition, MetaHash = metahash };
                        this.IndexStream.Write(newinfo.ToBytes(), 0, 24);
                        this.Docs[id] = newinfo;
                    }
                }
            }
        }

        public int AddIndex(long position, Guid MetaHash = default(Guid))
        {
            lock (_locker)
            {
                long currentposition = this.IndexStream.Length;
                // check to make sure it is the right position. 
                var left = currentposition % 24;
                if (left != 0)
                {
                    var count = (int)currentposition / 24;
                    if (count * 8 > currentposition)
                    { currentposition = count * 24; }
                    else
                    { currentposition = (count + 1) * 24; }
                    this.IndexStream.SetLength(currentposition);
                }

                this.IndexStream.Position = currentposition;
                DocInfo info = new DocInfo() { Position = position, MetaHash = MetaHash };

                this.IndexStream.Write(info.ToBytes(), 0, 24);
                int id = (int)(currentposition / 24);

                this.Docs[id] = info;
                return id;
            }
        }

        //TODO: this has very low performance now. 
        public int FindByMeta(string Meta)
        {
            Guid metahash = Lib.Security.Hash.ComputeGuidIgnoreCase(Meta);

            foreach (var item in this.Docs)
            {
                if (item.Value.MetaHash == metahash)
                {
                    return item.Key;
                }
            }
            return -1;
        }

        public void Close()
        {
            lock (_locker)
            {
                if (_indexstream != null)
                {
                    _indexstream.Close();
                    _indexstream.Dispose();
                    _indexstream = null;
                }

                if (_datastream != null)
                {
                    _datastream.Close();
                    _datastream.Dispose();
                    _datastream = null;
                }
            }

        }

        private FileStream _indexstream;

        private FileStream IndexStream
        {
            get
            {
                if (_indexstream == null)
                {
                    Lib.Helper.IOHelper.EnsureFileDirectoryExists(this.IndexFile);
                    _indexstream = File.Open(this.IndexFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                }
                return _indexstream;
            }
        }

        private FileStream _datastream;

        private FileStream DataStream
        {
            get
            {
                if (_datastream == null)
                {
                    Lib.Helper.IOHelper.EnsureFileDirectoryExists(this.DataFile);

                    _datastream = File.Open(this.DataFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

                    if (_datastream.Length == 0)
                    {
                        _datastream.Write(new byte[10], 0, 10);
                    }
                }
                return _datastream;
            }
        }

        public long DiskSize
        {
            get
            {
                return this.IndexStream.Length + this.DataStream.Length;
            }
        }
    }

    public class IndexDocument
    {
        public string Meta { get; set; }
        public string Body { get; set; }

        public int MetaLen { get; set; }

        public int BodyLen { get; set; }

        public int BodyBufferLen { get; set; }
    }

    public struct DocInfo
    {
        public long Position { get; set; }

        public Guid MetaHash { get; set; }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[24];
            Buffer.BlockCopy(BitConverter.GetBytes(this.Position), 0, bytes, 0, 8);
            byte[] guidbytes = Kooboo.IndexedDB.ByteConverter.GuidConverter.ConvertToByte(this.MetaHash);

            Buffer.BlockCopy(guidbytes, 0, bytes, 8, 16);
            return bytes;
        }

        public void FromBytes(byte[] bytes)
        {
            if (bytes.Length != 24)
            {
                throw new Exception("wrong byte length");
            }

            this.Position = BitConverter.ToInt64(bytes, 0);
            byte[] guidbytes = new byte[16];

            Buffer.BlockCopy(bytes, 8, guidbytes, 0, 16);

            this.MetaHash = Kooboo.IndexedDB.ByteConverter.GuidConverter.ConvertFromByte(guidbytes);

        }

    }
}