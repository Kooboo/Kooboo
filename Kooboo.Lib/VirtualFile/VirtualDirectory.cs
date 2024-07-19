namespace VirtualFile
{
    public class VirtualDirectory : IEntry
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

        public VirtualDirectory(string path, string source)
        {
            Path = path;
            Source = source;
        }
    }
}
