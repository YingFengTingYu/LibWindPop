using System;
using System.Diagnostics;

namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm
{
    public readonly struct DataFormat : IEquatable<DataFormat>
    {
        /// <summary>
        /// inner value
        /// </summary>
        private readonly uint value;

        public SurfaceFormat SurfaceFormat => (SurfaceFormat)((value >> 0) & 0xFFu);

        public TextureChannelType ChannelType => (TextureChannelType)((value >> 8) & 0xF);

        public TextureChannel ChannelX => (TextureChannel)((value >> 12) & 0x7);

        public TextureChannel ChannelY => (TextureChannel)((value >> 15) & 0x7);

        public TextureChannel ChannelZ => (TextureChannel)((value >> 18) & 0x7);

        public TextureChannel ChannelW => (TextureChannel)((value >> 21) & 0x7);

        public uint Unused => (value >> 24) & 0xFFu;

        private DataFormat(uint value)
        {
            this.value = value;
        }

        public DataFormat(SurfaceFormat m_surfaceFormat, TextureChannelType m_channelType, TextureChannel m_channelX, TextureChannel m_channelY, TextureChannel m_channelZ, TextureChannel m_channelW)
        {
            value = (((uint)m_surfaceFormat & 0xFFu) << 0)
                | (((uint)m_channelType & 0xFu) << 8)
                | (((uint)m_channelX & 0x7u) << 12)
                | (((uint)m_channelY & 0x7u) << 15)
                | (((uint)m_channelZ & 0x7u) << 18)
                | (((uint)m_channelW & 0x7u) << 21);
        }

        private DataFormat(SurfaceFormat m_surfaceFormat, TextureChannelType m_channelType, TextureChannel m_channelX, TextureChannel m_channelY, TextureChannel m_channelZ, TextureChannel m_channelW, uint m_unused)
        {
            Debug.Assert(m_unused == 0u);
            value = (((uint)m_surfaceFormat & 0xFFu) << 0)
                | (((uint)m_channelType & 0xFu) << 8)
                | (((uint)m_channelX & 0x7u) << 12)
                | (((uint)m_channelY & 0x7u) << 15)
                | (((uint)m_channelZ & 0x7u) << 18)
                | (((uint)m_channelW & 0x7u) << 21)
                | ((m_unused & 0xFFu) << 24);
        }

        public override bool Equals(object? obj)
        {
            return (obj is DataFormat fmt) && value == fmt.value;
        }

        public bool Equals(DataFormat other)
        {
            return value == other.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static bool operator ==(DataFormat fmt1, DataFormat fmt2)
        {
            return fmt1.value == fmt2.value;
        }

        public static bool operator !=(DataFormat fmt1, DataFormat fmt2)
        {
            return fmt1.value != fmt2.value;
        }

        public static implicit operator uint(DataFormat fmt)
        {
            return fmt.value;
        }

        public static implicit operator DataFormat(uint fmt)
        {
            return new DataFormat(fmt);
        }

        public static DataFormat kDataFormatInvalid => new DataFormat(SurfaceFormat.kSurfaceFormatInvalid, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR32G32B32A32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB32G32R32A32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR32G32B32X32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB32G32R32X32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR32G32B32A32Uint => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32_32, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR32G32B32A32Sint => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32_32, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR32G32B32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR32G32B32Uint => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR32G32B32Sint => new DataFormat(SurfaceFormat.kSurfaceFormat32_32_32, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR16G16B16A16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR16G16B16X16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB16G16R16X16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR16G16B16A16Uint => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR16G16B16A16Sint => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR16G16B16A16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB16G16R16A16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR16G16B16X16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB16G16R16X16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR16G16B16A16Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatL32A32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, 0);
        public static DataFormat kDataFormatR32G32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32_32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR32G32Uint => new DataFormat(SurfaceFormat.kSurfaceFormat32_32, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR32G32Sint => new DataFormat(SurfaceFormat.kSurfaceFormat32_32, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR11G11B10Float => new DataFormat(SurfaceFormat.kSurfaceFormat10_11_11, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8G8B8A8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR8G8B8X8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8G8B8A8UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR8G8B8X8UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8G8B8A8Uint => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR8G8B8A8Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR8G8B8A8Sint => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatL16A16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, 0);
        public static DataFormat kDataFormatR16G16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL16A16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, 0);
        public static DataFormat kDataFormatR16G16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR16G16Uint => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR16G16Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR16G16Sint => new DataFormat(SurfaceFormat.kSurfaceFormat16_16, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA32Float => new DataFormat(SurfaceFormat.kSurfaceFormat32, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);
        public static DataFormat kDataFormatR32Uint => new DataFormat(SurfaceFormat.kSurfaceFormat32, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR32Sint => new DataFormat(SurfaceFormat.kSurfaceFormat32, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR8G8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8G8Uint => new DataFormat(SurfaceFormat.kSurfaceFormat8_8, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR8G8Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8G8Sint => new DataFormat(SurfaceFormat.kSurfaceFormat8_8, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatL8A8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, 0);
        public static DataFormat kDataFormatL8A8UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormat8_8, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, 0);
        public static DataFormat kDataFormatR16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);
        public static DataFormat kDataFormatR16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA16Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);
        public static DataFormat kDataFormatR16Uint => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR16Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR16Sint => new DataFormat(SurfaceFormat.kSurfaceFormat16, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL8UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8Uint => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatR8Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR8Sint => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeSInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, 0);
        public static DataFormat kDataFormatA8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);

        public static DataFormat kDataFormatR1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat1, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat1, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat1, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);
        public static DataFormat kDataFormatR1Uint => new DataFormat(SurfaceFormat.kSurfaceFormat1, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL1Uint => new DataFormat(SurfaceFormat.kSurfaceFormat1, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA1Uint => new DataFormat(SurfaceFormat.kSurfaceFormat1, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);

        public static DataFormat kDataFormatR1ReversedUnorm => new DataFormat(SurfaceFormat.kSurfaceFormat1Reversed, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL1ReversedUnorm => new DataFormat(SurfaceFormat.kSurfaceFormat1Reversed, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA1ReversedUnorm => new DataFormat(SurfaceFormat.kSurfaceFormat1Reversed, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);
        public static DataFormat kDataFormatR1ReversedUint => new DataFormat(SurfaceFormat.kSurfaceFormat1Reversed, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatL1ReversedUint => new DataFormat(SurfaceFormat.kSurfaceFormat1Reversed, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatA1ReversedUint => new DataFormat(SurfaceFormat.kSurfaceFormat1Reversed, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelX, 0);

        public static DataFormat kDataFormatBc1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc1, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc1UBNorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc1, TextureChannelType.kTextureChannelTypeUBNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc1UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormatBc1, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc2Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc2, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc2UBNorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc2, TextureChannelType.kTextureChannelTypeUBNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc2UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormatBc2, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc3Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc3, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc3UBNorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc3, TextureChannelType.kTextureChannelTypeUBNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc3UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormatBc3, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc4Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc4, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc4Snorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc4, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc5Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc5, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc5Snorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc5, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelConstant0, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc6Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc6, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc6Snorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc6, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc6Uf16 => new DataFormat(SurfaceFormat.kSurfaceFormatBc6, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc6Sf16 => new DataFormat(SurfaceFormat.kSurfaceFormatBc6, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc7Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc7, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc7UBNorm => new DataFormat(SurfaceFormat.kSurfaceFormatBc7, TextureChannelType.kTextureChannelTypeUBNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatBc7UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormatBc7, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB5G6R5Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat5_6_5, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR5G5B5A1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat1_5_5_5, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB5G5R5A1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat1_5_5_5, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB8G8R8A8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB8G8R8X8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB8G8R8A8UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB8G8R8X8UnormSrgb => new DataFormat(SurfaceFormat.kSurfaceFormat8_8_8_8, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);

        public static DataFormat kDataFormatR4G4B4A4Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat4_4_4_4, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB4G4R4A4Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat4_4_4_4, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB4G4R4X4Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat4_4_4_4, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB5G5R5X1Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat1_5_5_5, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR5G6B5Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat5_6_5, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);

        public static DataFormat kDataFormatB10G10R10A2Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat2_10_10_10, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR10G10B10A2Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat2_10_10_10, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR10G10B10A2Uint => new DataFormat(SurfaceFormat.kSurfaceFormat2_10_10_10, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB10G10R10A2Uint => new DataFormat(SurfaceFormat.kSurfaceFormat2_10_10_10, TextureChannelType.kTextureChannelTypeUInt, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatB16G16R16A16Float => new DataFormat(SurfaceFormat.kSurfaceFormat16_16_16_16, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, 0);
        public static DataFormat kDataFormatR11G11B10Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat10_11_11, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatR11G11B10Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat10_11_11, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB10G11R11Unorm => new DataFormat(SurfaceFormat.kSurfaceFormat10_11_11, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB10G11R11Snorm => new DataFormat(SurfaceFormat.kSurfaceFormat10_11_11, TextureChannelType.kTextureChannelTypeSNorm, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelConstant1, 0);

        public static DataFormat kDataFormatR9G9B9E5Float => new DataFormat(SurfaceFormat.kSurfaceFormat5_9_9_9, TextureChannelType.kTextureChannelTypeFloat, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatB8G8R8G8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatBG_RG, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatG8B8G8R8Unorm => new DataFormat(SurfaceFormat.kSurfaceFormatGB_GR, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);

        public static DataFormat kDataFormatBc1UnormNoAlpha => new DataFormat(SurfaceFormat.kSurfaceFormatBc1, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc1UnormSrgbNoAlpha => new DataFormat(SurfaceFormat.kSurfaceFormatBc1, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);

        public static DataFormat kDataFormatBc7UnormNoAlpha => new DataFormat(SurfaceFormat.kSurfaceFormatBc7, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);
        public static DataFormat kDataFormatBc7UnormSrgbNoAlpha => new DataFormat(SurfaceFormat.kSurfaceFormatBc7, TextureChannelType.kTextureChannelTypeSrgb, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelY, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelConstant1, 0);

        public static DataFormat kDataFormatBc3UnormRABG => new DataFormat(SurfaceFormat.kSurfaceFormatBc3, TextureChannelType.kTextureChannelTypeUNorm, TextureChannel.kTextureChannelX, TextureChannel.kTextureChannelW, TextureChannel.kTextureChannelZ, TextureChannel.kTextureChannelY, 0);
    }
}
