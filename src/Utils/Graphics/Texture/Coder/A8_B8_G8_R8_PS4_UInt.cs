using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct A8_B8_G8_R8_PS4_UInt : ITextureCoder, IGnmTexture
    {
        public static DataFormat GnmFormat => DataFormat.kDataFormatR8G8B8A8Unorm;

        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            YFColor tempColor;
            uint colorU32;
            const int dataSizePerTexel = 4;
            int texelWidth = width;
            int texelHeight = height;
            (int alignWidth, int alignHeight) = PS4TextureHelper.GetAlignSizeFromTexelSize(texelWidth, texelHeight);
            int microTileWidth = Math.Min(8, texelWidth);
            int microTileHeight = Math.Min(8, texelHeight);
            for (int yMacroTile = 0; yMacroTile < alignHeight; yMacroTile += 8)
            {
                for (int xMacroTile = 0; xMacroTile < alignWidth; xMacroTile += 8)
                {
                    int baseTexelIndex = yMacroTile * alignWidth + xMacroTile * microTileHeight;
                    for (int yMicroTile = 0; yMicroTile < microTileHeight; yMicroTile++)
                    {
                        for (int xMicroTile = 0; xMicroTile < microTileWidth; xMicroTile++)
                        {
                            int texelIndex = baseTexelIndex + PS4TextureHelper.GetMicroTexelIndexFromPosition(xMicroTile, yMicroTile);
                            int dataIndex = texelIndex * dataSizePerTexel;
                            colorU32 = BinaryPrimitives.ReadUInt32LittleEndian(srcData.Slice(dataIndex, 4));
                            tempColor.Alpha = (byte)BitHelper.EightBitToEightBit(colorU32 >> 24);
                            tempColor.Blue = (byte)BitHelper.EightBitToEightBit(colorU32 >> 16);
                            tempColor.Green = (byte)BitHelper.EightBitToEightBit(colorU32 >> 8);
                            tempColor.Red = (byte)BitHelper.EightBitToEightBit(colorU32);
                            dstBitmap[xMacroTile + xMicroTile, yMacroTile + yMicroTile] = tempColor;
                        }
                    }
                }
            }
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            YFColor tempColor;
            uint colorU32;
            const int dataSizePerTexel = 4;
            int texelWidth = width;
            int texelHeight = height;
            (int alignWidth, int alignHeight) = PS4TextureHelper.GetAlignSizeFromTexelSize(texelWidth, texelHeight);
            int microTileWidth = Math.Min(8, texelWidth);
            int microTileHeight = Math.Min(8, texelHeight);
            for (int yMacroTile = 0; yMacroTile < alignHeight; yMacroTile += 8)
            {
                for (int xMacroTile = 0; xMacroTile < alignWidth; xMacroTile += 8)
                {
                    int baseTexelIndex = yMacroTile * alignWidth + xMacroTile * microTileHeight;
                    for (int yMicroTile = 0; yMicroTile < microTileHeight; yMicroTile++)
                    {
                        for (int xMicroTile = 0; xMicroTile < microTileWidth; xMicroTile++)
                        {
                            tempColor = srcBitmap[xMacroTile + xMicroTile, yMacroTile + yMicroTile];
                            colorU32 = (uint)((BitHelper.EightBitToEightBit(tempColor.Alpha) << 24) | (BitHelper.EightBitToEightBit(tempColor.Blue) << 16) | (BitHelper.EightBitToEightBit(tempColor.Green) << 8) | BitHelper.EightBitToEightBit(tempColor.Red));
                            int texelIndex = baseTexelIndex + PS4TextureHelper.GetMicroTexelIndexFromPosition(xMicroTile, yMicroTile);
                            int dataIndex = texelIndex * dataSizePerTexel;
                            BinaryPrimitives.WriteUInt32LittleEndian(dstData.Slice(dataIndex, 4), colorU32);
                        }
                    }
                }
            }
        }
    }
}
