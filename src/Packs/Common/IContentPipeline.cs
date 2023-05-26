using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Packs.Common
{
    public interface IContentPipeline
    {
        void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger);

        void OnEndBuild(string packPath, IFileSystem fileSystem, ILogger logger);

        void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger);
    }
}
