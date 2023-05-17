using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Text;

namespace LibWindPop.Packs.Rsb.ContentPipeline
{
    public sealed unsafe class NativeRsbContentPipeline : IRsbContentPipeline
    {
        private readonly Encoding m_encoding;
        private readonly delegate* unmanaged[Stdcall]<sbyte*, void> m_onStartBuild;
        private readonly delegate* unmanaged[Stdcall]<sbyte*, void> m_onEndBuild;
        private readonly delegate* unmanaged[Stdcall]<sbyte*, void> m_onAdd;

        public NativeRsbContentPipeline(Encoding encoding, delegate* unmanaged[Stdcall]<sbyte*, void> onStartBuild, delegate* unmanaged[Stdcall]<sbyte*, void> onEndBuild, delegate* unmanaged[Stdcall]<sbyte*, void> onAdd)
        {
            m_encoding = encoding;
            m_onStartBuild = onStartBuild;
            m_onEndBuild = onEndBuild;
            m_onAdd = onAdd;
        }

        public void OnStartBuild(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            if (m_onStartBuild != null)
            {
                // Create string span
                int maxByteCount = m_encoding.GetMaxByteCount(unpackPath.Length) + 1;
                Span<byte> buffer = stackalloc byte[maxByteCount];
                int byteCount = m_encoding.GetBytes(unpackPath, buffer);
                buffer[byteCount] = 0;
                fixed (byte* bytePtr = buffer)
                {
                    m_onStartBuild((sbyte*)bytePtr);
                }
            }
        }

        public void OnEndBuild(string rsbPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            if (m_onEndBuild != null)
            {
                // Create string span
                int maxByteCount = m_encoding.GetMaxByteCount(rsbPath.Length) + 1;
                Span<byte> buffer = stackalloc byte[maxByteCount];
                int byteCount = m_encoding.GetBytes(rsbPath, buffer);
                buffer[byteCount] = 0;
                fixed (byte* bytePtr = buffer)
                {
                    m_onEndBuild((sbyte*)bytePtr);
                }
            }
        }

        public void OnAdd(string unpackPath, IFileSystem fileSystem, ILogger logger, bool throwException)
        {
            if (m_onAdd != null)
            {
                // Create string span
                int maxByteCount = m_encoding.GetMaxByteCount(unpackPath.Length) + 1;
                Span<byte> buffer = stackalloc byte[maxByteCount];
                int byteCount = m_encoding.GetBytes(unpackPath, buffer);
                buffer[byteCount] = 0;
                fixed (byte* bytePtr = buffer)
                {
                    m_onAdd((sbyte*)bytePtr);
                }
            }
        }
    }
}
