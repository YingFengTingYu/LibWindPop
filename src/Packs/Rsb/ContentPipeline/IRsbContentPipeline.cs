using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public interface IRsbContentPipeline
    {
        void Build(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException);

        void Add(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException);
    }
}
