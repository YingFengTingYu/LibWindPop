using LibWindPop.Utils;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Logger;
using System;

namespace LibWindPop.Images.PtxRsb.Handler
{
    /// <summary>
    /// 植物大战僵尸2 v1.0.0
    /// </summary>
    public sealed class PtxHandlerPVZ2CNiOSV1 : IPtxRsbHandler
    {
        public bool UseExtend1AsAlphaSize => false;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            return GetPtxSizeWithoutAlpha(width, height, pitch, format);
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
                    return (width * height) >> 1;
                case 31u:
                    return (width * height) >> 2;
                default:
                    return pitch * height;
            }
        }

        public void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(B8_G8_R8_A8_UByte)}");
                    TextureCoder.Decode<B8_G8_R8_A8_UByte>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 1u:
                default:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(R4_G4_B4_A4_UShort)}");
                    TextureCoder.Decode<R4_G4_B4_A4_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_UShort)}");
                    TextureCoder.Decode<R5_G6_B5_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 3u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(R5_G5_B5_A1_UShort)}");
                    TextureCoder.Decode<R5_G5_B5_A1_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 21u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(R4_G4_B4_A4_Tile_UShort)}");
                    TextureCoder.Decode<R4_G4_B4_A4_Tile_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 22u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_Tile_UShort)}");
                    TextureCoder.Decode<R5_G6_B5_Tile_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 23u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(R5_G5_B5_A1_Tile_UShort)}");
                    TextureCoder.Decode<R5_G5_B5_A1_Tile_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 30u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(RGBA_PVRTCI_4BPP_UByte)}");
                    TextureCoder.Decode<RGBA_PVRTCI_4BPP_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 31u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(DecodePtx)} use format {nameof(RGBA_PVRTCI_4BPP_UByte)}");
                    TextureCoder.Decode<RGBA_PVRTCI_2BPP_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
            }
        }

        public void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(B8_G8_R8_A8_UByte)}");
                    TextureCoder.Encode<B8_G8_R8_A8_UByte>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 1u:
                default:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(R4_G4_B4_A4_UShort)}");
                    TextureCoder.Encode<R4_G4_B4_A4_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_UShort)}");
                    TextureCoder.Encode<R5_G6_B5_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 3u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(R5_G5_B5_A1_UShort)}");
                    TextureCoder.Encode<R5_G5_B5_A1_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 21u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(R4_G4_B4_A4_Tile_UShort)}");
                    TextureCoder.Encode<R4_G4_B4_A4_Tile_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 22u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_Tile_UShort)}");
                    TextureCoder.Encode<R5_G6_B5_Tile_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 23u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(R5_G5_B5_A1_Tile_UShort)}");
                    TextureCoder.Encode<R5_G5_B5_A1_Tile_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 30u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(RGBA_PVRTCI_4BPP_UByte)}");
                    TextureCoder.Encode<RGBA_PVRTCI_4BPP_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 31u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNiOSV1)}.{nameof(EncodePtx)} use format {nameof(RGBA_PVRTCI_2BPP_UByte)}");
                    TextureCoder.Encode<RGBA_PVRTCI_2BPP_UByte>(srcBitmap, ptxData, (int)width, (int)height);
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
                    if (BitHelper.IsPowerOfTwo(srcBitmap.Width) && BitHelper.IsPowerOfTwo(srcBitmap.Height))
                    {
                        width = (uint)srcBitmap.Width;
                        height = (uint)srcBitmap.Height;
                        pitch = width >> (format == 30u ? 1 : 2);
                        alphaSize = 0u;
                        return true;
                    }
                    break;
                default:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    pitch = width << (format == 0u ? 2 : 1);
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
