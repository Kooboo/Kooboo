namespace VirtualFile
{
    public interface IEntry
    {
        string Source { get; }

        string Path { get; }

        string Directory { get; }
    }
}
