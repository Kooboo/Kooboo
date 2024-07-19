//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Search.Scanner;

namespace Kooboo.Search
{
    public class Index
    {
        private object _locker = new object();

        public string Folder { get; set; }

        private WordIndex _wordindex;
        internal WordIndex WordIndex
        {
            get
            {
                if (_wordindex == null)
                {
                    lock (_locker)
                    {
                        if (_wordindex == null)
                        {
                            _wordindex = new WordIndex(this.Folder);
                        }
                    }
                }
                return _wordindex;
            }
            set
            {
                _wordindex = value;
            }
        }

        private DocumentStore _docstore;
        internal DocumentStore DocumentStore
        {
            get
            {
                if (_docstore == null)
                {
                    lock (_locker)
                    {
                        if (_docstore == null)
                        {
                            _docstore = new DocumentStore(this.Folder);
                        }
                    }
                }
                return _docstore;
            }
            set
            {
                _docstore = value;
            }
        }

        private BitMap _map;
        internal BitMap Map
        {
            get
            {
                if (_map == null)
                {
                    lock (_locker)
                    {
                        if (_map == null)
                        {
                            _map = new BitMap(this.Folder);
                        }
                    }
                }
                return _map;
            }
            set
            {
                _map = value;
            }
        }

        private Itokenizer _Tokenizer;
        public Itokenizer Tokenizer
        {
            get
            {
                if (_Tokenizer == null)
                {
                    lock (_locker)
                    {
                        if (_Tokenizer == null)
                        {
                            _Tokenizer = new Scanner.DefaultTokenizer();
                        }
                    }
                }
                return _Tokenizer;
            }
            set
            {
                _Tokenizer = value;
            }
        }

        private void init(string folder)
        {
            if (!System.IO.Path.IsPathRooted(folder))
            {
                this.Folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
            }
            else
            {
                this.Folder = folder;
            }
            Lib.Helper.IOHelper.EnsureDirectoryExists(this.Folder);
        }

        public Index(string folder)
        {
            init(folder);
        }
        public Index(string folder, Itokenizer Tokenizer)
        {
            this.Tokenizer = Tokenizer;
            init(folder);
        }

        public void Flush()
        {
            lock (_locker)
            {
                Map.WriteIndex();
            }
        }

        public string GetMeta(int id)
        {
            lock (_locker)
            {
                return this.DocumentStore.GetMeta(id);
            }
        }

        public IndexDocument LoadByMeta(string meta)
        {
            lock (_locker)
            {
                var id = this.DocumentStore.FindByMeta(meta);
                if (id > -1)
                {
                    return this.DocumentStore.Get(id);
                }
            }

            return null;
        }

        public HashSet<int> ListWords(string content)
        {
            HashSet<int> result = new HashSet<int>();

            var tokenizer = new DefaultTokenizer();
            tokenizer.SetDoc(content);
            var token = tokenizer.ConsumeNext();
            while (token != null)
            {
                var wordid = WordIndex.GetOrAddWord(token.Value);
                if (wordid >= 0)
                {
                    result.Add(wordid);
                }
                token = tokenizer.ConsumeNext();
            }
            return result;
        }

        public HashSet<int> ReadWords(string keywords)
        {
            HashSet<int> result = new HashSet<int>();

            var tokenizer = new DefaultTokenizer();
            tokenizer.SetDoc(keywords);
            var token = tokenizer.ConsumeNext();
            while (token != null)
            {
                var wordid = WordIndex.GetWord(token.Value);
                if (wordid != -1)
                {
                    result.Add(wordid);
                }
                token = tokenizer.ConsumeNext();
            }
            return result;
        }

        /// <summary>
        /// search and return list of metas
        /// </summary>
        /// <param name="FullText"></param>
        /// <returns></returns>
        public List<string> Search(string FullText)
        {
            lock (_locker)
            {
                var FoundIds = FindAll(FullText);

                List<string> result = new List<string>();

                foreach (var item in FoundIds)
                {
                    var meta = this.DocumentStore.GetMeta(item);
                    if (meta != null)
                    {
                        result.Add(meta);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// read only find, only for kooboo search. 
        /// </summary>
        /// <param name="FullText"></param>
        /// <returns></returns>
        public List<int> FindAll(string FullText)
        {
            lock (_locker)
            {
                var wordlist = ReadWords(FullText);
                return Map.FindAll(wordlist, true);
            }
        }

        public void Delete(string MetaKey)
        {
            lock (_locker)
            {
                var find = this.DocumentStore.FindByMeta(MetaKey);

                if (find > -1)
                {
                    var doc = this.DocumentStore.Get(find);
                    if (doc != null)
                    {
                        var oldwordlist = ListWords(doc.Body);
                        this.Map.Update(find, oldwordlist, new HashSet<int>());
                    }
                    this.DocumentStore.Delete(find);
                }

            }
        }

        public void AddOrUpdate(string metakey, string body)
        {
            lock (_locker)
            {
                var find = this.DocumentStore.FindByMeta(metakey);
                if (find > -1)
                {
                    var doc = this.DocumentStore.Get(find);
                    HashSet<int> oldwords = new HashSet<int>();
                    if (doc != null)
                    { oldwords = ListWords(doc.Body); }
                    this.DocumentStore.Update(find, body);
                    HashSet<int> newwords = ListWords(body);
                    this.Map.Update(find, oldwords, newwords);
                }
                else
                {
                    var words = ListWords(body);
                    if (words.Any())
                    {
                        var id = this.DocumentStore.Add(body, metakey);
                        this.Map.Add(id, words);
                    }
                }
            }
        }

        public void Add(string body, string metakey)
        {
            lock (_locker)
            {
                int docid = DocumentStore.Add(body, metakey, true);
                var worklist = ListWords(body);
                Map.Add(docid, worklist);
            }
        }

        public void Close()
        {
            lock (_locker)
            {
                this.DocumentStore.Close();
                this.DocumentStore = null;
                this.Map.Close();
                this.Map = null;
                this.WordIndex.Close();
                this.WordIndex = null;
            }
        }

        public void DelSelf()
        {
            lock (_locker)
            {
                this.Close();
                if (System.IO.Directory.Exists(this.Folder))
                {
                    System.IO.Directory.Delete(this.Folder, true);
                }
            }
        }

        public IndexStat GetIndexStat()
        {
            lock (_locker)
            {
                IndexStat stat = new IndexStat();
                stat.WordCount = this.WordIndex.Words.Count;
                stat.DocCount = this.DocumentStore.Docs.Count;
                stat.DiskSize = this.WordIndex.DiskSize + this.DocumentStore.DiskSize + this.Map.DiskSize;
                return stat;
            }
        }
    }

    public class IndexStat
    {
        public int WordCount { get; set; }
        public int DocCount { get; set; }

        public long DiskSize { get; set; }

        public bool EnableFullTextSearch { get; set; }
    }
}
