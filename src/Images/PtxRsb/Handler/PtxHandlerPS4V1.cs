using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Logger;
using System;

namespace LibWindPop.Images.PtxRsb.Handler
{
    /// <summary>
    /// Peggle 2 (I do not have PS4. All the code related to GNM has not been tested)
    /// </summary>
    public sealed class PtxHandlerPS4V1 : IPtxRsbHandler
    {
        public bool UseExtend1AsAlphaSize => false;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            return GetPtxSizeWithoutAlpha(width, height, pitch, format);
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            // from Peggle 2 PS4 0x5FCA10(Sexy::OrbisResStreamsDriver::GetGPUDataSizeForTexture) (Need to create function by yourself)
            if (format == 4u || format == 5u)
            {
                return ((height + 3u) & 0xFFFFFFFCu) * pitch;
            }
            return pitch * height;
        }

        public void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 0u: // 0xFAC00A
                default:
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(DecodePtx)} use format {nameof(A8_B8_G8_R8_PS4_UInt)}", 0);
                    TextureCoder.Decode<A8_B8_G8_R8_PS4_UInt>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 1u: // 0xF2E013
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(DecodePtx)} use format {nameof(A4_R4_G4_B4_PS4_UShort)}", 0);
                    TextureCoder.Decode<A4_R4_G4_B4_PS4_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 2u: // 0x32E010
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_PS4_UShort)}", 0);
                    TextureCoder.Decode<R5_G6_B5_PS4_UShort>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 4u: // 0xFAC023
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(DecodePtx)} use format {nameof(RGBA_BC1_PS4_UByte)}", 0);
                    TextureCoder.Decode<RGBA_BC1_PS4_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 5u: // 0xFAC025
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(DecodePtx)} use format {nameof(RGBA_BC3_PS4_UByte)}", 0);
                    TextureCoder.Decode<RGBA_BC3_PS4_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
            }
        }

        public void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 0u: // 0xFAC00A
                default:
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(EncodePtx)} use format {nameof(A8_B8_G8_R8_PS4_UInt)}", 0);
                    TextureCoder.Encode<A8_B8_G8_R8_PS4_UInt>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 1u: // 0xF2E013
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(EncodePtx)} use format {nameof(A4_R4_G4_B4_PS4_UShort)}", 0);
                    TextureCoder.Encode<A4_R4_G4_B4_PS4_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 2u: // 0x32E010
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_PS4_UShort)}", 0);
                    TextureCoder.Encode<R5_G6_B5_PS4_UShort>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 4u: // 0xFAC023
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(EncodePtx)} use format {nameof(RGBA_BC1_PS4_UByte)}", 0);
                    TextureCoder.Encode<RGBA_BC1_PS4_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 5u: // 0xFAC025
                    logger.Log($"{nameof(PtxHandlerPS4V1)}.{nameof(EncodePtx)} use format {nameof(RGBA_BC3_PS4_UByte)}", 0);
                    TextureCoder.Encode<RGBA_BC3_PS4_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
            }
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            if (format == 4u || format == 5u)
            {
                width = (uint)srcBitmap.Width;
                height = (uint)srcBitmap.Height;
                uint size;
                if (width < 32 && height < 32)
                {
                    uint border = width;
                    if (height > width)
                    {
                        border = height;
                    }
                    size = border * border;
                }
                else
                {
                    uint tileWidth = width, tileHeight = height;
                    if ((tileWidth & 31u) != 0u)
                    {
                        tileWidth |= 31u;
                        tileWidth++;
                    }
                    if ((tileHeight & 31u) != 0u)
                    {
                        tileHeight |= 31u;
                        tileHeight++;
                    }
                    size = tileWidth * tileHeight;
                }
                if (format == 4u)
                {
                    size >>= 1;
                }
                uint alignHeight = height;
                if ((alignHeight & 3u) != 0u)
                {
                    alignHeight |= 3u;
                    alignHeight++;
                }
                pitch = (size + alignHeight - 1) / alignHeight;
                alphaSize = 0u;
                return true;
            }
            else
            {
                width = (uint)srcBitmap.Width;
                height = (uint)srcBitmap.Height;
                uint size;
                if (width < 8 && height < 8)
                {
                    uint border = width;
                    if (height > width)
                    {
                        border = height;
                    }
                    size = border * border;
                }
                else
                {
                    uint tileWidth = width, tileHeight = height;
                    if ((tileWidth & 7u) != 0u)
                    {
                        tileWidth |= 7u;
                        tileWidth++;
                    }
                    if ((tileHeight & 7u) != 0u)
                    {
                        tileHeight |= 7u;
                        tileHeight++;
                    }
                    size = tileWidth * tileHeight;
                }
                size <<= (format == 1u || format == 2u) ? 1 : 2;
                pitch = (size + height - 1) / height;
                alphaSize = 0u;
                return true;
            }
        }
    }
}
