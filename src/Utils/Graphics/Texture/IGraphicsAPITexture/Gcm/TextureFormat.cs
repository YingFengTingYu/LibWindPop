using System;

namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gcm
{
    public readonly struct TextureFormat : IEquatable<TextureFormat>
    {
        /// <summary>
        /// inner value
        /// </summary>
        private readonly uint value;

        public CellGcmEnum Location => (CellGcmEnum)(((value >> 0) & 0x3u) - 1);

        public CellGcmEnum CubeMap => (CellGcmEnum)((value >> 2) & 0x1u);

        public CellGcmEnum Border => (CellGcmEnum)((value >> 3) & 0x1u);

        public CellGcmEnum Dimension => (CellGcmEnum)((value >> 4) & 0xFu);

        public CellGcmEnum Format => (CellGcmEnum)((value >> 8) & 0xFFu);

        public uint Mipmap => (value >> 16) & 0xFu;

        private TextureFormat(uint value)
        {
            this.value = value;
        }

        /// <summary>
        /// CELL_GCM_METHOD_DATA_TEXTURE_FORMAT(location, cubemap, dimension, format, mipmap)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cubemap"></param>
        /// <param name="dimension"></param>
        /// <param name="format"></param>
        /// <param name="mipmap"></param>
        public TextureFormat(CellGcmEnum location, CellGcmEnum cubemap, CellGcmEnum dimension, CellGcmEnum format, uint mipmap)
        {
            value = ((((uint)location + 1) & 0x3u) << 0)
                | (((uint)cubemap & 0x1u) << 2)
                | (((uint)CellGcmEnum.CELL_GCM_TEXTURE_BORDER_COLOR & 0x1u) << 3)
                | (((uint)dimension & 0xFu) << 4)
                | (((uint)format & 0xFFu) << 8)
                | ((mipmap & 0xFu) << 16);
        }

        /// <summary>
        /// CELL_GCM_METHOD_DATA_TEXTURE_BORDER_FORMAT(location, cubemap, dimension, format, mipmap, border)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cubemap"></param>
        /// <param name="dimension"></param>
        /// <param name="format"></param>
        /// <param name="mipmap"></param>
        /// <param name="border"></param>
        public TextureFormat(CellGcmEnum location, CellGcmEnum cubemap, CellGcmEnum dimension, CellGcmEnum format, uint mipmap, CellGcmEnum border)
        {
            value = (((uint)(location + 1) & 0x3u) << 0)
                | (((uint)cubemap & 0x1u) << 2)
                | (((uint)border & 0x1u) << 3)
                | (((uint)dimension & 0xFu) << 4)
                | (((uint)format & 0xFFu) << 8)
                | ((mipmap & 0xFu) << 16);
        }

        public readonly override bool Equals(object? obj)
        {
            return (obj is TextureFormat fmt) && value == fmt.value;
        }

        public readonly bool Equals(TextureFormat other)
        {
            return value == other.value;
        }

        public readonly override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }

        public static bool operator ==(TextureFormat fmt1, TextureFormat fmt2)
        {
            return fmt1.value == fmt2.value;
        }

        public static bool operator !=(TextureFormat fmt1, TextureFormat fmt2)
        {
            return fmt1.value != fmt2.value;
        }

        public static implicit operator uint(TextureFormat fmt)
        {
            return fmt.value;
        }

        public static implicit operator TextureFormat(uint fmt)
        {
            return new TextureFormat(fmt);
        }
    }
}
