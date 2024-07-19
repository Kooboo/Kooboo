namespace Kooboo.IndexedDB
{
    public class FilePart
    {
        public string FullFileName { get; set; }

        public long BlockPosition { get; set; }

        public long RelativePosition { get; set; }

        public long Length { get; set; }
    }
}
