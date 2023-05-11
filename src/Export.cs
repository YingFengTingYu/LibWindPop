﻿using LibWindPop.Utils;
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
        public static int RsbUnpack(sbyte* rsbPath, sbyte* unpackPath, sbyte* ptxHandlerType, int logLevel, int useGroupFolder, int throwException)
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
        public static int RsbUnpackU8(sbyte* rsbPath, sbyte* unpackPath, sbyte* ptxHandlerType, int logLevel, int useGroupFolder, int throwException)
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
        public static int RsbAddContentPipeline(sbyte* unpackPath, sbyte* pipelineName, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.RsbContentPipelineManager.AddContentPipeline(
                GetStringFromPtr(unpackPath, m_Ansi),
                GetStringFromPtr(pipelineName, m_Ansi),
                new NativeFileSystem(),
                new ConsoleLogger(logLevel),
                throwException != 0
                ));
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(RsbAddContentPipelineU8))]
        public static int RsbAddContentPipelineU8(sbyte* unpackPath, sbyte* pipelineName, int logLevel, int throwException)
        {
            return PCall(() => Packs.Rsb.RsbContentPipelineManager.AddContentPipeline(
                GetStringFromPtr(unpackPath, m_utf8),
                GetStringFromPtr(pipelineName, m_utf8),
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

        [UnmanagedCallersOnly(EntryPoint = nameof(GetErrorSize))]
        public static nuint GetErrorSize()
        {
            return (nuint)PeekPtrSizeFromString(m_errorMessage, m_Ansi);
        }

        [UnmanagedCallersOnly(EntryPoint = nameof(GetErrorSizeU8))]
        public static nuint GetErrorSizeU8()
        {
            return (nuint)PeekPtrSizeFromString(m_errorMessage, m_utf8);
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