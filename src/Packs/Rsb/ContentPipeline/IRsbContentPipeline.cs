using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public interface IRsbContentPipeline
    {
        void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException);

        void OnEndBuild(string rsbPath, IFileSystem fileSystem, ILogger logger, bool throwException);

        void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException);
    }
}
