using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace LibWindPop.Utils.Extension
{
    internal static unsafe class StreamExtension
    {
        public static byte ReadUInt8(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[1];
            stream.Read(buffer);
            return buffer[0];
        }

        public static ushort ReadUInt16LE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.Read(buffer);
            return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        }

        public static ushort ReadUInt16BE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.Read(buffer);
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }

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

        public static ulong ReadUInt64LE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.Read(buffer);
            return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
        }

        public static ulong ReadUInt64BE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.Read(buffer);
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }

        public static sbyte ReadInt8(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[1];
            stream.Read(buffer);
            return (sbyte)buffer[0];
        }

        public static short ReadInt16LE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.Read(buffer);
            return BinaryPrimitives.ReadInt16LittleEndian(buffer);
        }

        public static short ReadInt16BE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[2];
            stream.Read(buffer);
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
        }

        public static int ReadInt32LE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.Read(buffer);
            return BinaryPrimitives.ReadInt32LittleEndian(buffer);
        }

        public static int ReadInt32BE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[4];
            stream.Read(buffer);
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        public static long ReadInt64LE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.Read(buffer);
            return BinaryPrimitives.ReadInt64LittleEndian(buffer);
        }

        public static long ReadInt64BE(this Stream stream)
        {
            Span<byte> buffer = stackalloc byte[8];
            stream.Read(buffer);
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
        }

        public static string ReadString(this Stream stream, int size, Encoding encoding)
        {
            if (size <= 0)
            {
                return string.Empty;
            }
            if (size <= 0x800)
            {
                Span<byte> buffer = stackalloc byte[size];
                stream.Read(buffer);
                return encoding.GetString(buffer);
            }
            using (NativeMemoryOwner owner = new NativeMemoryOwner((uint)size))
            {
                Span<byte> buffer = owner.AsSpan();
                stream.Read(buffer);
                return encoding.GetString(buffer);
            }
        }

        public static void WriteUInt8(this Stream stream, byte value)
        {
            Span<byte> buffer = stackalloc byte[1];
            buffer[0] = value;
            stream.Write(buffer);
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
