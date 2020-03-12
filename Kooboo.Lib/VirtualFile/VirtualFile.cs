using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VirtualFile
{
    public abstract class VirtualFile : IEntry
    {
        string _directory = null;

        public string Path { get; }

        public string Source { get; }

        public string Directory
        {
            get
            {
                if (_directory == null)
                {
                    _directory = System.IO.Path.GetDirectoryName(Path);
                }

                return _directory;
            }
        }

        protected VirtualFile(string path, string source)
        {
            Path = path;
            Source = source;
        }

        public abstract Stream Open();

        public abstract byte[] ReadAllBytes();

        public abstract string ReadAllText();
    }
}
