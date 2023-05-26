using LibWindPop.Utils;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Logger;
using System;
using A8_UByte = LibWindPop.Images.PtxRsb.AlphaCoder.A8_UByte;

namespace LibWindPop.Images.PtxRsb.Handler
{
    /// <summary>
    /// Plants vs Zombies 2 v1.5; Plants vs Zombies 2 v10.5.2
    /// </summary>
    public sealed class PtxHandleriOSV5 : IPtxRsbHandler
    {
        public bool UseExtend1AsAlphaSize => false;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            uint tex0Size = GetPtxSizeWithoutAlpha(width, height, pitch, format);
            return format switch
            {
                147u or 148u or 149u => tex0Size + width * height,
                _ => tex0Size,
            };
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            switch (format)
            {
                case 21u:
                case 22u:
                case 23u:
                    if ((height & 0x1Fu) != 0u)
                    {
                        height |= 0x1Fu;
                        height++;
                    }
                    return pitch * height;
                case 30u:
                case 32u:
                case 35u:
                case 147u:
                case 148u:
                    return (width * height) >> 1;
                case 31u:
                    return (width * height) >> 2;
                case 36u:
                case 37u:
                case 39u:
                    return width * height;
                case 38u:
                    return (3 * width * height) >> 2;
                case 149u:
                    return (width * height) << 2;
                default:
                    return pitch * height;
            }
        }

        public void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(B8_G8_R8_A8_UByte)}");
                    TextureCoder.Decode<B8_G8_R8_A8_UByte>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 1u:
                default:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(R4_G4_B4_A4_UShort)}");
                    TextureCoder.Decode<R4_G4_B4_A4_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_UShort)}");
                    TextureCoder.Decode<R5_G6_B5_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 3u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(R5_G5_B5_A1_UShort)}");
                    TextureCoder.Decode<R5_G5_B5_A1_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 21u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(R4_G4_B4_A4_Tile_UShort)}");
                    TextureCoder.Decode<R4_G4_B4_A4_Tile_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 22u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_Tile_UShort)}");
                    TextureCoder.Decode<R5_G6_B5_Tile_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 23u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(R5_G5_B5_A1_Tile_UShort)}");
                    TextureCoder.Decode<R5_G5_B5_A1_Tile_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 30u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(RGBA_PVRTCI_4BPP_UByte)}");
                    TextureCoder.Decode<RGBA_PVRTCI_4BPP_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 31u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(RGBA_PVRTCI_4BPP_UByte)}");
                    TextureCoder.Decode<RGBA_PVRTCI_2BPP_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 148u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(RGB_PVRTCI_4BPP_UByte)} and {nameof(A8_UByte)}");
                    int tex0Size_148 = (int)GetPtxSizeWithoutAlpha(width, height, pitch, format);
                    TextureCoder.Decode<RGB_PVRTCI_4BPP_UByte>(ptxData[..tex0Size_148], (int)width, (int)height, dstBitmap);
                    A8_UByte.Decode(ptxData[tex0Size_148..], (int)width, (int)height, dstBitmap);
                    break;
                case 149u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(DecodePtx)} use format {nameof(B8_G8_R8_A8_UByte)} and {nameof(A8_UByte)}");
                    int tex0Size_149 = (int)GetPtxSizeWithoutAlpha(width, height, pitch, format);
                    TextureCoder.Decode<B8_G8_R8_A8_UByte>(ptxData[..tex0Size_149], (int)width, (int)height, dstBitmap);
                    A8_UByte.Decode(ptxData[tex0Size_149..], (int)width, (int)height, dstBitmap);
                    break;
            }
        }

        public void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(B8_G8_R8_A8_UByte)}");
                    TextureCoder.Encode<B8_G8_R8_A8_UByte>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 1u:
                default:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(R4_G4_B4_A4_UShort)}");
                    TextureCoder.Encode<R4_G4_B4_A4_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_UShort)}");
                    TextureCoder.Encode<R5_G6_B5_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 3u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(R5_G5_B5_A1_UShort)}");
                    TextureCoder.Encode<R5_G5_B5_A1_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 21u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(R4_G4_B4_A4_Tile_UShort)}");
                    TextureCoder.Encode<R4_G4_B4_A4_Tile_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 22u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_Tile_UShort)}");
                    TextureCoder.Encode<R5_G6_B5_Tile_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 23u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(R5_G5_B5_A1_Tile_UShort)}");
                    TextureCoder.Encode<R5_G5_B5_A1_Tile_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 30u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(RGBA_PVRTCI_4BPP_UByte)}");
                    TextureCoder.Encode<RGBA_PVRTCI_4BPP_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 31u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(RGBA_PVRTCI_2BPP_UByte)}");
                    TextureCoder.Encode<RGBA_PVRTCI_2BPP_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 148u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(RGB_PVRTCI_4BPP_UByte)} and {nameof(A8_UByte)}");
                    int tex0Size_148 = (int)GetPtxSizeWithoutAlpha(width, height, pitch, format);
                    TextureCoder.Encode<RGB_PVRTCI_4BPP_UByte>(srcBitmap, ptxData[..tex0Size_148], (int)width, (int)height);
                    A8_UByte.Encode(srcBitmap, ptxData[tex0Size_148..], (int)width, (int)height);
                    break;
                case 149u:
                    logger.Log($"{nameof(PtxHandleriOSV5)}.{nameof(EncodePtx)} use format {nameof(B8_G8_R8_A8_UByte)} and {nameof(A8_UByte)}");
                    int tex0Size_149 = (int)GetPtxSizeWithoutAlpha(width, height, pitch, format);
                    TextureCoder.Encode<B8_G8_R8_A8_UByte>(srcBitmap, ptxData[..tex0Size_149], (int)width, (int)height);
                    A8_UByte.Encode(srcBitmap, ptxData[tex0Size_149..], (int)width, (int)height);
                    break;
            }
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            switch (format)
            {
                case 21u:
                case 22u:
                case 23u:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    uint align32Width = width;
                    if ((align32Width & 0x1Fu) != 0u)
                    {
                        align32Width |= 0x1Fu;
                        align32Width++;
                    }
                    pitch = align32Width << 1;
                    alphaSize = 0;
                    return true;
                case 30u:
                case 31u:
                case 148u:
                    if (BitHelper.IsPowerOfTwo(srcBitmap.Width) && BitHelper.IsPowerOfTwo(srcBitmap.Height))
                    {
                        width = (uint)srcBitmap.Width;
                        height = (uint)srcBitmap.Height;
                        pitch = format switch
                        {
                            30u => width >> 1,
                            31u => width >> 2,
                            _ => width << 2
                        };
                        alphaSize = 0u;
                        return true;
                    }
                    break;
                default:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    pitch = width << ((format == 0u || format == 149u) ? 2 : 1);
                    alphaSize = 0u;
                    return true;
            }
            width = 0u;
            height = 0u;
            pitch = 0u;
            alphaSize = 0u;
            return false;
        }
    }
}
