using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.Gnm;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;
using System.Buffers.Binary;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public unsafe struct A4_R4_G4_B4_PS4_UShort : ITextureCoder, IGnmTexture
    {
        public static DataFormat GnmFormat => DataFormat.kDataFormatB4G4R4A4Unorm;

        public void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            YFColor tempColor;
            ushort colorU16;
            const int dataSizePerTexel = 2;
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
                            colorU16 = BinaryPrimitives.ReadUInt16LittleEndian(srcData.Slice(dataIndex, 2));
                            tempColor.Alpha = (byte)BitHelper.FourBitToEightBit(colorU16 >> 12);
                            tempColor.Red = (byte)BitHelper.FourBitToEightBit(colorU16 >> 8);
                            tempColor.Green = (byte)BitHelper.FourBitToEightBit(colorU16 >> 4);
                            tempColor.Blue = (byte)BitHelper.FourBitToEightBit(colorU16);
                            dstBitmap[xMacroTile + xMicroTile, yMacroTile + yMicroTile] = tempColor;
                        }
                    }
                }
            }
        }

        public void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            YFColor tempColor;
            ushort colorU16;
            const int dataSizePerTexel = 2;
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
                            colorU16 = (ushort)((BitHelper.EightBitToFourBit(tempColor.Alpha) << 12) | (BitHelper.EightBitToFourBit(tempColor.Red) << 8) | (BitHelper.EightBitToFourBit(tempColor.Green) << 4) | BitHelper.EightBitToFourBit(tempColor.Blue));
                            int texelIndex = baseTexelIndex + PS4TextureHelper.GetMicroTexelIndexFromPosition(xMicroTile, yMicroTile);
                            int dataIndex = texelIndex * dataSizePerTexel;
                            BinaryPrimitives.WriteUInt16LittleEndian(dstData.Slice(dataIndex, 2), colorU16);
                        }
                    }
                }
            }
        }
    }
}
