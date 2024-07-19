//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Search
{
    public class WordIndex
    {
        private object _locker = new object();

        internal string FullFileName { get; set; }

        public Dictionary<string, int> Words { get; set; }

        public int CurrentIndex { get; set; }
        private Encoding Encoding { get; set; }

        public WordIndex(string Folder, Encoding Encoding = null)
        {
            if (Encoding == null)
            {
                this.Encoding = Encoding.UTF8;
            }
            else
            {
                this.Encoding = Encoding;
            }

            this.CurrentIndex = 0;
            if (Folder != null)
            {
                Lib.Helper.IOHelper.EnsureDirectoryExists(Folder);
                this.FullFileName = System.IO.Path.Combine(Folder, "word.dat");
            }
            this.Words = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            LoadWordList(this.FullFileName);
        }

        internal void LoadWordList(string fileName = null)
        {
            if (fileName == null)
            {
                fileName = this.FullFileName;
            }

            if (fileName != null && File.Exists(fileName))
            {
                var allBytes = Lib.Helper.IOHelper.ReadAllBytes(fileName);

                int totalLen = allBytes.Length;
                int maxstartindex = totalLen - 7;

                int index = 0;

                while (index < maxstartindex)
                {
                    var byteone = allBytes[index];
                    var bytetwo = allBytes[index + 1];

                    // if (not set, go till next 10; 
                    if (byteone != 10 || bytetwo != 13)
                    {
                        bool found = false;
                        for (int i = index; i < maxstartindex; i++)
                        {
                            if (allBytes[i] == 10)
                            {
                                index = i;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    var bytelen = allBytes[index + 2];
                    int seglen = -1;
                    if (bytelen == 127)
                    {
                        var wordlen = BitConverter.ToInt16(allBytes, index + 3);
                        seglen = wordlen + 9;
                    }
                    else
                    {
                        seglen = bytelen + 7;
                    }

                    if (index + seglen > totalLen)
                    {
                        break;
                    }

                    byte[] segbytes = new byte[seglen];

                    Buffer.BlockCopy(allBytes, index, segbytes, 0, seglen);

                    var value = FromByte(segbytes);

                    if (!String.IsNullOrEmpty(value.Key))
                    {
                        this.Words[value.Key] = value.Value;
                        if (CurrentIndex < value.Value)
                        {
                            CurrentIndex = value.Value;
                        }
                    }

                    index = index + seglen;
                }

            }

        }

        public int GetOrAddWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return -1;
            }

            if (this.Words.ContainsKey(word))
            {
                return this.Words[word];
            }

            lock (_locker)
            {
                CurrentIndex += 1;
                this.Words[word] = CurrentIndex;
                WriteToWordFile(word, CurrentIndex);
                return CurrentIndex;
            }
        }

        public int GetWord(string word)
        {
            if (!string.IsNullOrEmpty(word) && this.Words.ContainsKey(word))
            {
                return this.Words[word];
            }
            return -1;
        }

        public HashSet<int> ListWordIndex(List<string> words)
        {
            HashSet<int> result = new HashSet<int>();

            foreach (var item in words)
            {
                var wordindex = GetOrAddWord(item);
                if (wordindex >= 0)
                {
                    result.Add(wordindex);
                }
            }
            return result;
        }

        public byte[] ToBytes(string word, int index)
        {
            byte[] wordbytes = this.Encoding.GetBytes(word);

            byte[] indexbytes = BitConverter.GetBytes(index);

            int len = wordbytes.Length;
            if (len < 127)
            {
                int totallen = len + 7;
                byte[] bytes = new byte[totallen];
                bytes[0] = 10;
                bytes[1] = 13;

                bytes[2] = (byte)len;

                Buffer.BlockCopy(indexbytes, 0, bytes, 3, 4);

                Buffer.BlockCopy(wordbytes, 0, bytes, 7, len);
                return bytes;
            }
            else
            {
                int totallen = len + 9;
                byte[] bytes = new byte[totallen];
                bytes[0] = 10;
                bytes[1] = 13;

                bytes[2] = 127;
                Int16 len16 = (Int16)len;

                Buffer.BlockCopy(BitConverter.GetBytes(len16), 0, bytes, 3, 2);

                Buffer.BlockCopy(indexbytes, 0, bytes, 5, 4);

                Buffer.BlockCopy(wordbytes, 0, bytes, 9, len);

                return bytes;
            }
        }

        public KeyValuePair<string, int> FromByte(byte[] segmentbytes)
        {
            if (segmentbytes == null)
            {
                return new KeyValuePair<string, int>();
            }
            int seglen = segmentbytes.Length;
            if (seglen < 8)
            {
                return new KeyValuePair<string, int>();
            }

            if (segmentbytes[0] == 10 && segmentbytes[1] == 13)
            {
                Int16 wordlen;
                byte len = segmentbytes[2];
                int index = -1;

                string word = null;

                if (len == 127)
                {
                    wordlen = BitConverter.ToInt16(segmentbytes, 3);
                    if (seglen < wordlen + 9)
                    {
                        return new KeyValuePair<string, int>();
                    }
                    index = BitConverter.ToInt32(segmentbytes, 5);
                    word = Encoding.GetString(segmentbytes, 9, wordlen);
                }
                else
                {
                    wordlen = (Int16)len;

                    if (seglen < wordlen + 7)
                    {
                        return new KeyValuePair<string, int>();
                    }

                    index = BitConverter.ToInt32(segmentbytes, 3);
                    word = Encoding.GetString(segmentbytes, 7, wordlen);
                }

                if (index > -1 && !string.IsNullOrEmpty(word))
                {
                    return new KeyValuePair<string, int>(word, index);
                }
            }
            return new KeyValuePair<string, int>();
        }

        private void WriteToWordFile(string word, int index)
        {
            var bytes = ToBytes(word, index);
            this.Stream.Position = this.Stream.Length;
            this.Stream.Write(bytes, 0, bytes.Length);
        }

        public void Close()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
        }

        private FileStream _stream;

        internal FileStream Stream
        {
            get
            {
                if (_stream == null)
                {
                    Lib.Helper.IOHelper.EnsureFileDirectoryExists(FullFileName);
                    _stream = File.Open(FullFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
                }
                return _stream;
            }
        }

        public long DiskSize
        {
            get { return this.Stream.Length; }
        }
    }
}
