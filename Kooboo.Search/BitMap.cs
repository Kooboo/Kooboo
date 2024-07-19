//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Search
{
    public class BitMap
    {
        // naming convention... main.{id}.data.  batch.{id}.data;  
        private object _locker = new object();
        private Dictionary<int, BitArray> Changes = new Dictionary<int, BitArray>();

        private string _filepath;
        private int FileNumber { get; set; } = 0;

        private string GetFilePath(int filenumber = -1)
        {
            if (filenumber == -1)
            {
                filenumber = this.FileNumber;
            }
            string path = "main." + filenumber.ToString() + ".dat";
            path = System.IO.Path.Combine(this.Folder, path);
            return path;
        }

        public string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_filepath))
                {
                    _filepath = GetFilePath();
                }
                return _filepath;
            }
            set
            {
                _filepath = value;
            }
        }

        public string Folder { get; set; }

        private long _currentsize;
        public long CurrentSize
        {
            get
            {
                if (_currentsize == 0)
                {
                    return Setting.Size;
                }
                else
                {
                    return _currentsize;
                }
            }
            set
            {
                _currentsize = value;
            }
        }

        public BitMap(string Folder)
        {
            this.Folder = Folder;
            this.Init();
        }

        public BitMap(string Folder, long size)
        {
            this.Folder = Folder;
            this.CurrentSize = size;
            this.Init();
        }

        private void Init()
        {
            Lib.Helper.IOHelper.EnsureDirectoryExists(Folder);
            var allfiles = Directory.GetFiles(this.Folder, "main.*.dat");

            if (allfiles.Count() > 0)
            {
                string file = null;
                int number = -1;

                foreach (var item in allfiles)
                {
                    FileInfo info = new FileInfo(item);

                    string strnumber = info.Name.Replace("main.", "").Replace(".dat", "");
                    int currentnumber = int.Parse(strnumber);
                    if (currentnumber > number)
                    {
                        file = info.FullName;
                        number = currentnumber;
                    }
                }

                if (!string.IsNullOrEmpty(file))
                {
                    this.FilePath = file;
                    FileNumber = number;
                }
            }
            LoadStream();
        }

        private FileStream _stream;

        private FileStream IndexStream
        {
            get
            {
                LoadStream();
                return _stream;
            }
            set
            {
                _stream = value;
            }
        }

        private void LoadStream()
        {
            if (_stream == null)
            {
                lock (_locker)
                {
                    if (_stream == null)
                    {
                        FileStream stream;
                        if (!File.Exists(this.FilePath))
                        {
                            Lib.Helper.IOHelper.EnsureFileDirectoryExists(FilePath);

                            byte[] initbytes = new byte[10];
                            initbytes[0] = 10;
                            initbytes[1] = 13;
                            Buffer.BlockCopy(BitConverter.GetBytes(CurrentSize), 0, initbytes, 2, 8);
                            stream = File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                            stream.Write(initbytes, 0, 10);
                        }
                        else
                        {
                            stream = File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);

                            byte[] sizebytes = new byte[8];
                            stream.Position = 2;
                            stream.Read(sizebytes, 0, 8);
                            this.CurrentSize = BitConverter.ToInt64(sizebytes, 0);
                        }
                        _stream = stream;
                    }
                }
            }
        }

        // wordid start with 0. 
        public void Add(int docId, HashSet<int> wordids)
        {
            lock (_locker)
            {
                EnsureSize(wordids.Max());

                if (docId < this.CurrentSize)
                {
                    //still writting to main 
                    foreach (var item in wordids)
                    {
                        BitArray bitarray;
                        if (Changes.ContainsKey(item))
                        {
                            bitarray = Changes[item];
                        }
                        else
                        {
                            bitarray = this.LoadDisk(item);
                            Changes.Add(item, bitarray);
                        }
                        bitarray.Set(docId, true);
                    }

                    // if condition, write to disk.  
                    // TO BE IMPROVED for performance.
                    WriteIndex();

                }
                else
                {
                    // reorganize the index.... 
                    IncreaseSize();
                    Add(docId, wordids);
                }
            }
        }

        public void Update(int docId, HashSet<int> oldIds, HashSet<int> newIds)
        {
            List<int> removeitem = oldIds.Where(o => !newIds.Contains(o)).ToList();
            List<int> additems = newIds.Where(o => !oldIds.Contains(o)).ToList();
            if (removeitem.Any())
            {
                this.Remove(docId, new HashSet<int>(removeitem));
            }
            if (additems.Any())
            {
                this.Add(docId, new HashSet<int>(additems));
            }
        }

        public void Remove(int docId, HashSet<int> wordids)
        {
            lock (_locker)
            {
                EnsureSize(wordids.Max());

                if (docId < this.CurrentSize)
                {
                    foreach (var item in wordids)
                    {
                        BitArray bitarray;
                        if (Changes.ContainsKey(item))
                        {
                            bitarray = Changes[item];
                        }
                        else
                        {
                            bitarray = this.LoadDisk(item);
                            Changes.Add(item, bitarray);
                        }
                        bitarray.Set(docId, false);
                    }
                    // if condition, write to disk.  
                    WriteIndex();
                }
            }
        }

        internal void IncreaseSize(long increased = -1)
        {
            if (increased == -1)
            { increased = Setting.Size; }

            int newnumber = this.FileNumber + 1;
            string filename = GetFilePath(newnumber);

            long newsize = this.CurrentSize + increased;

            byte[] initbytes = new byte[10];
            initbytes[0] = 10;
            initbytes[1] = 13;
            Buffer.BlockCopy(BitConverter.GetBytes(newsize), 0, initbytes, 2, 8);
            var newstream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            newstream.Write(initbytes, 0, 10);

            var currentlen = this.IndexStream.Length;
            var currentBlockByteLen = (int)CurrentSize / 8;
            var currentrecords = (int)(currentlen - 10) / currentBlockByteLen;

            var newblocklen = (int)newsize / 8;
            var newtotallen = currentrecords * newblocklen;
            newstream.SetLength(newtotallen);

            for (int i = 0; i < currentrecords; i++)
            {
                var oldposition = i * currentBlockByteLen + 10;
                byte[] oldbyte = new byte[currentBlockByteLen];

                var newposition = i * newblocklen + 10;

                byte[] newbyte = new byte[newblocklen];

                this.IndexStream.Position = oldposition;
                this.IndexStream.Read(oldbyte, 0, currentBlockByteLen);

                Buffer.BlockCopy(oldbyte, 0, newbyte, 0, currentBlockByteLen);

                newstream.Position = newposition;
                newstream.Write(newbyte, 0, newblocklen);
            }

            var oldstream = this.IndexStream;
            var oldfile = this.FilePath;

            this.FileNumber = newnumber;
            this.FilePath = filename;
            this.CurrentSize = newsize;
            this.IndexStream = newstream;
            oldstream.Close();
            File.Delete(oldfile);
        }

        // return document ids. 
        public List<int> FindAll(HashSet<int> wordids, bool sort = false)
        {
            List<BitArray> sets = new List<BitArray>();
            foreach (var item in wordids)
            {
                var bitResult = Load(item);
                if (bitResult != null)
                {
                    sets.Add(bitResult);
                }
            }

            if (!sets.Any())
            {
                return new List<int>();
            }

            BitArray bitarray = null;

            foreach (var item in sets)
            {
                if (bitarray == null)
                {
                    bitarray = item;
                }
                else
                {
                    bitarray = bitarray.Or(item);
                }
            }

            var result = GetBitTrueValues(bitarray);

            if (sort)
            {
                Dictionary<int, int> idscroes = new Dictionary<int, int>();

                int score = 0;

                foreach (var item in result)
                {
                    foreach (var set in sets)
                    {
                        if (set[item])
                        {
                            score += 1;
                        }
                    }
                    idscroes.Add(item, score);
                }
                return idscroes.OrderByDescending(o => o.Value).Select(o => o.Key).ToList();
            }

            return result;
        }

        internal List<int> GetBitTrueValues(BitArray array)
        {
            List<int> result = new List<int>();
            int len = array.Length;

            for (int i = len - 1; i >= 0; i--)
            {
                if (array[i])
                {
                    result.Add(i);
                }
            }
            return result;
        }

        internal void WriteIndex()
        {
            if (this.Changes.Any())
            {
                int bytelen = (int)this.CurrentSize / 8;
                foreach (var item in Changes)
                {
                    var array = item.Value;
                    byte[] bytevalue = new byte[bytelen];

                    item.Value.CopyTo(bytevalue, 0);

                    int positin = item.Key * bytelen + 10;
                    this.IndexStream.Position = positin;
                    this.IndexStream.Write(bytevalue, 0, bytelen);
                }

                this.Changes.Clear();
                this.IndexStream.Flush();
            }
        }

        internal BitArray Load(int wordid)
        {
            if (this.Changes.ContainsKey(wordid))
            {
                return this.Changes[wordid];
            }
            else
            {
                return LoadDisk(wordid);
            }
        }

        internal BitArray LoadDisk(int wordid)
        {
            int bytelen = (int)this.CurrentSize / 8;
            long position = wordid * bytelen + 10;
            byte[] arraybytes = new byte[bytelen];
            lock (_locker)
            {
                this.IndexStream.Position = position;
                this.IndexStream.Read(arraybytes, 0, bytelen);
            }
            return new BitArray(arraybytes);
        }

        private void EnsureSize(int max)
        {
            var currentlength = this.IndexStream.Length;
            var maxlen = this.CurrentSize / 8 * (max + 1) + 10;
            if (currentlength < maxlen)
            {
                this.IndexStream.SetLength(maxlen);
            }
        }

        public void Close()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }
        }
        public long DiskSize
        {
            get
            {
                return this.IndexStream.Length;
            }
        }
    }

    public struct sortResult
    {
        public int DocId { get; set; }

        public int Score { get; set; }
    }


}
