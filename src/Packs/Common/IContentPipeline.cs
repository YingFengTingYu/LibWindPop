using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Packs.Common
{
    public interface IContentPipeline
    {
        void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException);

        void OnEndBuild(string packPath, IFileSystem fileSystem, ILogger logger, bool throwException);

        void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException);
    }
}
