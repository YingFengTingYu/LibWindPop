using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Utils;
using LibWindPop.Utils.Extension;
using LibWindPop.Utils.FileSystem;
using LibWindPop.Utils.Logger;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LibWindPop
{
    public static unsafe class Export
    {
        private static string? m_errorMessage;
        private static readonly Encoding m_utf8 = EncodingType.utf_8.GetEncoding();
        private static readonly Encoding m_Ansi = EncodingType.ansi.GetEncoding();

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbUnpack))]
        public static int RsbUnpack(sbyte* rsbPath, sbyte* unpackPath, sbyte* ptxHandlerType, int useGroupFolder, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.RsbUnpacker.Unpack(
                GetStringFromPtr(rsbPath, m_Ansi),
                GetStringFromPtr(unpackPath, m_Ansi),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                GetStringFromPtr(ptxHandlerType, m_Ansi),
                useGroupFolder != 0,
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbUnpackU8))]
        public static int RsbUnpackU8(sbyte* rsbPath, sbyte* unpackPath, sbyte* ptxHandlerType, int useGroupFolder, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.RsbUnpacker.Unpack(
                GetStringFromPtr(rsbPath, m_utf8),
                GetStringFromPtr(unpackPath, m_utf8),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                GetStringFromPtr(ptxHandlerType, m_utf8),
                useGroupFolder != 0,
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbAddContentPipeline))]
        public static int RsbAddContentPipeline(sbyte* unpackPath, sbyte* pipelineName, int atFirst, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.ContentPipeline.RsbContentPipelineManager.AddContentPipeline(
                GetStringFromPtr(unpackPath, m_Ansi),
                GetStringFromPtr(pipelineName, m_Ansi),
                atFirst != 0,
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbAddContentPipelineU8))]
        public static int RsbAddContentPipelineU8(sbyte* unpackPath, sbyte* pipelineName, int atFirst, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.ContentPipeline.RsbContentPipelineManager.AddContentPipeline(
                GetStringFromPtr(unpackPath, m_utf8),
                GetStringFromPtr(pipelineName, m_utf8),
                atFirst != 0,
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbPack))]
        public static int RsbPack(sbyte* unpackPath, sbyte* rsbPath, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.RsbPacker.Pack(
                GetStringFromPtr(unpackPath, m_Ansi),
                GetStringFromPtr(rsbPath, m_Ansi),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbPackU8))]
        public static int RsbPackU8(sbyte* unpackPath, sbyte* rsbPath, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.RsbPacker.Pack(
                GetStringFromPtr(unpackPath, m_utf8),
                GetStringFromPtr(rsbPath, m_utf8),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbRegistContentPipeline))]
        public static int RsbRegistContentPipeline(sbyte* pipelineName, delegate* unmanaged[Stdcall]<sbyte*, void> onStartBuild, delegate* unmanaged[Stdcall]<sbyte*, void> onEndBuild, delegate* unmanaged[Stdcall]<sbyte*, void> onAdd)
        {
            return PCall(() => Packs.Rsb.ContentPipeline.RsbContentPipelineManager.RegistPipeline(
                GetStringFromPtr(pipelineName, m_Ansi),
                new Packs.Rsb.ContentPipeline.NativeRsbContentPipeline(m_Ansi, onStartBuild, onEndBuild, onAdd)
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbRegistContentPipelineU8))]
        public static int RsbRegistContentPipelineU8(sbyte* pipelineName, delegate* unmanaged[Stdcall]<sbyte*, void> onStartBuild, delegate* unmanaged[Stdcall]<sbyte*, void> onEndBuild, delegate* unmanaged[Stdcall]<sbyte*, void> onAdd)
        {
            return PCall(() => Packs.Rsb.ContentPipeline.RsbContentPipelineManager.RegistPipeline(
                GetStringFromPtr(pipelineName, m_utf8),
                new Packs.Rsb.ContentPipeline.NativeRsbContentPipeline(m_utf8, onStartBuild, onEndBuild, onAdd)
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(PtxRsbDecode))]
        public static int PtxRsbDecode(sbyte* ptxPath, sbyte* pngPath, sbyte* ptxHandlerType, int width, int height, int pitch, int format, int alphaSize, int logLevel)
        {
            return PCall(() => Images.PtxRsb.PtxCoder.Decode(
                GetStringFromPtr(ptxPath, m_Ansi),
                GetStringFromPtr(pngPath, m_Ansi),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                (uint)width,
                (uint)height,
                (uint)pitch,
                (uint)format,
                (uint)alphaSize,
                GetStringFromPtr(ptxHandlerType, m_Ansi)
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(PtxRsbDecodeU8))]
        public static int PtxRsbDecodeU8(sbyte* ptxPath, sbyte* pngPath, sbyte* ptxHandlerType, int width, int height, int pitch, int format, int alphaSize, int logLevel)
        {
            return PCall(() => Images.PtxRsb.PtxCoder.Decode(
                GetStringFromPtr(ptxPath, m_utf8),
                GetStringFromPtr(pngPath, m_utf8),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                (uint)width,
                (uint)height,
                (uint)pitch,
                (uint)format,
                (uint)alphaSize,
                GetStringFromPtr(ptxHandlerType, m_utf8)
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(PtxRsbEncode))]
        public static int PtxRsbEncode(sbyte* pngPath, sbyte* ptxPath, sbyte* ptxHandlerType, int format, int logLevel, int* width, int* height, int* pitch, int* alphaSize)
        {
            return PCall(() =>
            {
                Images.PtxRsb.PtxCoder.Encode(
                    GetStringFromPtr(pngPath, m_Ansi),
                    GetStringFromPtr(ptxPath, m_Ansi),
                    new NativeFileSystem(),
                    new ConsoleLogger(logLevel),
                    (uint)format,
                    out uint tWidth,
                    out uint tHeight,
                    out uint tPitch,
                    out uint tAlphaSize,
                    GetStringFromPtr(ptxHandlerType, m_Ansi)
                    );
                *width = (int)tWidth;
                *height = (int)tHeight;
                *pitch = (int)tPitch;
                *alphaSize = (int)tAlphaSize;
            });
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(PtxRsbEncodeU8))]
        public static int PtxRsbEncodeU8(sbyte* pngPath, sbyte* ptxPath, sbyte* ptxHandlerType, int format, int logLevel, int* width, int* height, int* pitch, int* alphaSize)
        {
            return PCall(() =>
            {
                Images.PtxRsb.PtxCoder.Encode(
                    GetStringFromPtr(pngPath, m_utf8),
                    GetStringFromPtr(ptxPath, m_utf8),
                    new NativeFileSystem(),
                    new ConsoleLogger(logLevel),
                    (uint)format,
                    out uint tWidth,
                    out uint tHeight,
                    out uint tPitch,
                    out uint tAlphaSize,
                    GetStringFromPtr(ptxHandlerType, m_utf8)
                    );
                *width = (int)tWidth;
                *height = (int)tHeight;
                *pitch = (int)tPitch;
                *alphaSize = (int)tAlphaSize;
            });
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(PtxRsbRegistHandler))]
        public static int PtxRsbRegistHandler(sbyte* ptxHandlerType, int useExtend1AsAlphaSize, delegate* unmanaged[Stdcall]<int, int, int, int, int, int> getPtxSize, delegate* unmanaged[Stdcall]<int, int, int, int, int> getPtxSizeWithoutAlpha, delegate* unmanaged[Stdcall]<void*, int, void*, int, int, int, int, int, int, int, void> decodePtx, delegate* unmanaged[Stdcall]<void*, int, int, void*, int, int, int, int, int, int, void> encodePtx, delegate* unmanaged[Stdcall]<void*, int, int, int, int*, int*, int*, int*, int> peekEncodedPtxInfo)
        {
            return PCall(() => Images.PtxRsb.PtxRsbHandlerManager.RegistHandler(
                GetStringFromPtr(ptxHandlerType, m_Ansi),
                new NativePtxHandler(useExtend1AsAlphaSize != 0, getPtxSize, getPtxSizeWithoutAlpha, decodePtx, encodePtx, peekEncodedPtxInfo)
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(PtxRsbRegistHandlerU8))]
        public static int PtxRsbRegistHandlerU8(sbyte* ptxHandlerType, int useExtend1AsAlphaSize, delegate* unmanaged[Stdcall]<int, int, int, int, int, int> getPtxSize, delegate* unmanaged[Stdcall]<int, int, int, int, int> getPtxSizeWithoutAlpha, delegate* unmanaged[Stdcall]<void*, int, void*, int, int, int, int, int, int, int, void> decodePtx, delegate* unmanaged[Stdcall]<void*, int, int, void*, int, int, int, int, int, int, void> encodePtx, delegate* unmanaged[Stdcall]<void*, int, int, int, int*, int*, int*, int*, int> peekEncodedPtxInfo)
        {
            return PCall(() => Images.PtxRsb.PtxRsbHandlerManager.RegistHandler(
                GetStringFromPtr(ptxHandlerType, m_utf8),
                new NativePtxHandler(useExtend1AsAlphaSize != 0, getPtxSize, getPtxSizeWithoutAlpha, decodePtx, encodePtx, peekEncodedPtxInfo)
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(GetErrorSize))]
        public static int GetErrorSize()
        {
            return PeekPtrSizeFromString(m_errorMessage, m_Ansi);
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(GetErrorSizeU8))]
        public static int GetErrorSizeU8()
        {
            return PeekPtrSizeFromString(m_errorMessage, m_utf8);
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(GetError))]
        public static int GetError(sbyte* buffer)
        {
            return PCall(() =>
            {
                GetPtrFromString(m_errorMessage, buffer, m_Ansi);
            });
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(GetErrorU8))]
        public static int GetErrorU8(sbyte* buffer)
        {
            return PCall(() =>
            {
                GetPtrFromString(m_errorMessage, buffer, m_utf8);
            });
        }

        private static int PCall(Action action)
        {
            try
            {
                action?.Invoke();
                return 0;
            }
            catch (Exception ex)
            {
                m_errorMessage = ex.Message;
                return 1;
            }
        }

        private static string GetStringFromPtr(sbyte* buffer, Encoding encoding)
        {
            // Marshal.PtrToStringAnsi only return UTF8 string
            ArgumentNullException.ThrowIfNull(buffer, nameof(buffer));
            byte* bufferAsBytePtr = (byte*)buffer;
            return encoding.GetString(bufferAsBytePtr, String_H.StrLen(bufferAsBytePtr));
        }

        private static int PeekPtrSizeFromString(string? str, Encoding encoding)
        {
            return 1 + (string.IsNullOrEmpty(str) ? 0 : encoding.GetByteCount(str));
        }

        private static void GetPtrFromString(string? str, sbyte* buffer, Encoding encoding)
        {
            int mSize = PeekPtrSizeFromString(str, encoding) - 1;
            if (!string.IsNullOrEmpty(str))
            {
                encoding.GetBytes(str, new Span<byte>(buffer, mSize));
            }
            buffer[mSize] = 0;
        }
    }
}
