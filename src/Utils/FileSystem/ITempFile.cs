using System;
using System.IO;

namespace LibWindPop.Utils.FileSystem
{
    public interface ITempFile : IDisposable
    {
        string NativePath { get; }

        Stream Stream { get; }
    }
}
