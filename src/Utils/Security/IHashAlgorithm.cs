using System;

namespace LibWindPop.Utils.Security
{
    public interface IHashAlgorithm
    {
        int ByteSize { get; }

        void Init();

        void Update(byte data)
        {
            unsafe
            {
                ReadOnlySpan<byte> dataSpan = new ReadOnlySpan<byte>(&data, 1);
                Update(dataSpan);
            }
        }

        void Update(ReadOnlySpan<byte> data);
    }
}
