using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualFile
{
    public interface IEntry
    {
        string Source { get; }

        string Path { get; }

        string Directory { get; }
    }
}
