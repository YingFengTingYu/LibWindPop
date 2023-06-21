using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture.Shared;
using System;

namespace LibWindPop.Utils.Graphics.Texture.Coder
{
    public readonly unsafe struct RGBA_BC1_PS4_UByte : ITextureCoder
    {
        public readonly void Decode(ReadOnlySpan<byte> srcData, int width, int height, RefBitmap dstBitmap)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            Span<YFColor> decodeBlockData = stackalloc YFColor[16];
            const int dataSizePerTexel = 8;
            int texelWidth = (width + 3) / 4;
            int texelHeight = (height + 3) / 4;
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
                            BCCoder.DecodeBC1BlockWithAlpha(srcData.Slice(dataIndex, dataSizePerTexel), decodeBlockData);
                            int texelY = yMacroTile + yMicroTile;
                            int texelX = xMacroTile + xMicroTile;
                            for (int y = 0; y < 4; y++)
                            {
                                int textureY = (texelY << 2) | y;
                                if (textureY < height)
                                {
                                    Span<YFColor> rowData = dstBitmap[textureY];
                                    for (int x = 0; x < 4; x++)
                                    {
                                        int textureX = (texelX << 2) | x;
                                        if (textureX < width)
                                        {
                                            rowData[textureX] = decodeBlockData[(y << 2) | x];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public readonly void Encode(RefBitmap srcBitmap, Span<byte> dstData, int width, int height)
        {
            ThrowHelper.ThrowWhen(width < 0 || height < 0);
            Span<YFColor> encodeBlockData = stackalloc YFColor[16];
            const int dataSizePerTexel = 8;
            int texelWidth = (width + 3) / 4;
            int texelHeight = (height + 3) / 4;
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
                            encodeBlockData.Fill(YFColor.Transparent);
                            int texelY = yMacroTile + yMicroTile;
                            int texelX = xMacroTile + xMicroTile;
                            for (int y = 0; y < 4; y++)
                            {
                                int textureY = (texelY << 2) | y;
                                if (textureY < height)
                                {
                                    Span<YFColor> rowData = srcBitmap[textureY];
                                    for (int x = 0; x < 4; x++)
                                    {
                                        int textureX = (texelX << 2) | x;
                                        if (textureX < width)
                                        {
                                            encodeBlockData[(y << 2) | x] = rowData[textureX];
                                        }
                                    }
                                }
                            }
                            int texelIndex = baseTexelIndex + PS4TextureHelper.GetMicroTexelIndexFromPosition(xMicroTile, yMicroTile);
                            int dataIndex = texelIndex * dataSizePerTexel;
                            BCCoder.EncodeBC1BlockWithAlpha(dstData.Slice(dataIndex, dataSizePerTexel), encodeBlockData);
                        }
                    }
                }
            }
        }
    }
}
