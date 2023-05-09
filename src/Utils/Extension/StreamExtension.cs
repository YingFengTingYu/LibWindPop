using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;

namespace LibWindPop.Utils.Extension
{
    internal static unsafe class StreamExtension
    {
        public static uint ReadUInt32LE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.Read(buffer);
            return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
        }

        public static uint ReadUInt32BE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.Read(buffer);
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }

        public static void WriteUInt32LE(this Stream stream, uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
            stream.Write(buffer);
        }

        public static void WriteUInt32BE(this Stream stream, uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            stream.Write(buffer);
        }

        public static long CopyLengthTo(this Stream src, Stream destination, long len)
        {
            const long BUFFER_LEN = 81920;
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)BUFFER_LEN);
            try
            {
                long readLen = 0;
                long bytesRead;
                while (readLen < len)
                {
                    bytesRead = src.Read(buffer, 0, (int)Math.Min(BUFFER_LEN, len - readLen));
                    if (bytesRead > 0)
                    {
                        destination.Write(buffer, 0, (int)Math.Min(bytesRead, len - readLen));
                        readLen += bytesRead;
                    }
                    else
                    {
                        return readLen;
                    }
                }
                return readLen;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static nuint Read(this Stream stream, void* Pointer, nuint Length)
        {
            const nuint BUFFERSIZE = 0x40000000;
            nuint readLength = 0x0;
            int currentReadLength;
            while ((Length - readLength) >= BUFFERSIZE)
            {
                currentReadLength = stream.Read(new Span<byte>((void*)((nuint)Pointer + readLength), (int)BUFFERSIZE));
                readLength += (nuint)currentReadLength;
                if ((nuint)currentReadLength != BUFFERSIZE)
                {
                    return readLength;
                }
            }
            if (readLength != Length)
            {
                readLength += (nuint)stream.Read(new Span<byte>((void*)((nuint)Pointer + readLength), (int)(Length - readLength)));
            }
            return readLength;
        }

        public static nuint Read(this Stream stream, nuint Pointer, nuint Length)
        {
            const nuint BUFFERSIZE = 0x40000000;
            nuint readLength = 0x0;
            int currentReadLength;
            while ((Length - readLength) >= BUFFERSIZE)
            {
                currentReadLength = stream.Read(new Span<byte>((void*)(Pointer + readLength), (int)BUFFERSIZE));
                readLength += (nuint)currentReadLength;
                if ((nuint)currentReadLength != BUFFERSIZE)
                {
                    return readLength;
                }
            }
            if (readLength != Length)
            {
                readLength += (nuint)stream.Read(new Span<byte>((void*)(Pointer + readLength), (int)(Length - readLength)));
            }
            return readLength;
        }

        public static void Write(this Stream stream, void* Pointer, nuint Length)
        {
            const nuint BUFFERSIZE = 0x40000000;
            nuint writeLength = 0x0;
            while ((Length - writeLength) >= BUFFERSIZE)
            {
                stream.Write(new ReadOnlySpan<byte>((void*)((nuint)Pointer + writeLength), (int)BUFFERSIZE));
                writeLength += BUFFERSIZE;
            }
            if (writeLength != Length)
            {
                stream.Write(new ReadOnlySpan<byte>((void*)((nuint)Pointer + writeLength), (int)(Length - writeLength)));
            }
        }

        public static void Write(this Stream stream, nuint Pointer, nuint Length)
        {
            const nuint BUFFERSIZE = 0x40000000;
            nuint writeLength = 0x0;
            while ((Length - writeLength) >= BUFFERSIZE)
            {
                stream.Write(new ReadOnlySpan<byte>((void*)(Pointer + writeLength), (int)BUFFERSIZE));
                writeLength += BUFFERSIZE;
            }
            if (writeLength != Length)
            {
                stream.Write(new ReadOnlySpan<byte>((void*)(Pointer + writeLength), (int)(Length - writeLength)));
            }
        }
    }
}
