using LibWindPop.Images.PtxRsb.AlphaCoder;
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
    /// 植物大战僵尸2高清版 v6 （安卓v4.1.1版本后）
    /// </summary>
    public sealed class PtxHandlerPVZ2CNAndroidV6 : IPtxRsbHandler
    {
        private readonly PtxHandlerPVZ2CNAndroidV5 m_v5 = new PtxHandlerPVZ2CNAndroidV5();

        public bool UseExtend1AsAlphaSize => m_v5.UseExtend1AsAlphaSize;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            if (format is >= 160u and <= 163u)
            {
                return GetPtxSizeWithoutAlpha(width, height, pitch, format);
            }
            return m_v5.GetPtxSize(width, height, pitch, format, alphaSize);
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            switch (format)
            {
                case 160u:
                    if (PtxRsbHandlerManager.FIX_COMPRESSED_TEX_SIZE)
                    {
                        if ((width & 0x3u) != 0u)
                        {
                            width |= 0x3u;
                            width++;
                        }
                        if ((height & 0x3u) != 0u)
                        {
                            height |= 0x3u;
                            height++;
                        }
                    }
                    return width * height;
                case 161u:
                    if (PtxRsbHandlerManager.FIX_COMPRESSED_TEX_SIZE)
                    {
                        if ((width % 5u) != 0u)
                        {
                            width = ((width / 5u) + 1u) * 5u;
                        }
                        if ((height % 5u) != 0u)
                        {
                            height = ((height / 5u) + 1u) * 5u;
                        }
                    }
                    return (width / 5u) * (height / 5u) * 16u;
                case 162u:
                    if (PtxRsbHandlerManager.FIX_COMPRESSED_TEX_SIZE)
                    {
                        if ((width % 6u) != 0u)
                        {
                            width = ((width / 6u) + 1u) * 6u;
                        }
                        if ((height % 6u) != 0u)
                        {
                            height = ((height / 6u) + 1u) * 6u;
                        }
                    }
                    return (width / 6u) * (height / 6u) * 16u;
                case 163u:
                    if (PtxRsbHandlerManager.FIX_COMPRESSED_TEX_SIZE)
                    {
                        if ((width & 0x7u) != 0u)
                        {
                            width |= 0x7u;
                            width++;
                        }
                        if ((height & 0x7u) != 0u)
                        {
                            height |= 0x7u;
                            height++;
                        }
                    }
                    return (width * height) >> 2;
                default:
                    return m_v5.GetPtxSizeWithoutAlpha(width, height, pitch, format);
            }
        }

        public void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 160u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(DecodePtx)} use format {nameof(RGBA_ASTC_4x4_UByte)}");
                    TextureCoder.Decode<RGBA_ASTC_4x4_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 161u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(DecodePtx)} use format {nameof(RGBA_ASTC_5x5_UByte)}");
                    TextureCoder.Decode<RGBA_ASTC_5x5_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 162u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(DecodePtx)} use format {nameof(RGBA_ASTC_6x6_UByte)}");
                    TextureCoder.Decode<RGBA_ASTC_6x6_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                case 163u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(DecodePtx)} use format {nameof(RGBA_ASTC_8x8_UByte)}");
                    TextureCoder.Decode<RGBA_ASTC_8x8_UByte>(ptxData, (int)width, (int)height, dstBitmap);
                    break;
                default:
                    m_v5.DecodePtx(ptxData, dstBitmap, width, height, pitch, format, alphaSize, logger);
                    break;
            }
        }

        public void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            switch (format)
            {
                case 160u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(EncodePtx)} use format {nameof(RGBA_ASTC_4x4_UByte)}");
                    TextureCoder.Encode<RGBA_ASTC_4x4_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 161u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(EncodePtx)} use format {nameof(RGBA_ASTC_5x5_UByte)}");
                    TextureCoder.Encode<RGBA_ASTC_5x5_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 162u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(EncodePtx)} use format {nameof(RGBA_ASTC_6x6_UByte)}");
                    TextureCoder.Encode<RGBA_ASTC_6x6_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                case 163u:
                    logger.Log($"{nameof(PtxHandlerPVZ2CNAndroidV6)}.{nameof(EncodePtx)} use format {nameof(RGBA_ASTC_8x8_UByte)}");
                    TextureCoder.Encode<RGBA_ASTC_8x8_UByte>(srcBitmap, ptxData, (int)width, (int)height);
                    break;
                default:
                    m_v5.EncodePtx(srcBitmap, ptxData, width, height, pitch, format, alphaSize, logger);
                    break;
            }
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            switch (format)
            {
                case 160u:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    pitch = width; // bpp = 1.0
                    alphaSize = 0u;
                    return true;
                case 161u:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    pitch = width * 16u / 25u; // bpp = 16/25 ≈ 0.64
                    alphaSize = 0u;
                    return true;
                case 162u:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    pitch = width * 4u / 9u; // bpp = 4/9 ≈ 0.44
                    alphaSize = 0u;
                    return true;
                case 163u:
                    width = (uint)srcBitmap.Width;
                    height = (uint)srcBitmap.Height;
                    pitch = width >> 2; // bpp = 0.25
                    alphaSize = 0u;
                    return true;
                default:
                    return m_v5.PeekEncodedPtxInfo(srcBitmap, format, out width, out height, out pitch, out alphaSize);
            }
        }
    }
}
