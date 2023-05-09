using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Logger;
using System;

namespace LibWindPop.Images.PtxRsb.Handler
{
    /// <summary>
    /// Bejeweled Classic v1.0; Allied Star Police v1.0
    /// </summary>
    public sealed class PtxHandleriOSV1 : IPtxRsbHandler
    {
        public bool UseExtend1AsAlphaSize => false;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            return GetPtxSizeWithoutAlpha(width, height, pitch, format);
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            return pitch * height;
        }

        public void DecodePtx<TLogger>(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, TLogger logger)
            where TLogger : ILogger
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandleriOSV1)}.{nameof(DecodePtx)} use format {nameof(B8_G8_R8_A8_UByte)}", 0);
                    TextureCoder.Decode<B8_G8_R8_A8_UByte>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 1u:
                default:
                    logger.Log($"{nameof(PtxHandleriOSV1)}.{nameof(DecodePtx)} use format {nameof(R4_G4_B4_A4_UShort)}", 0);
                    TextureCoder.Decode<R4_G4_B4_A4_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandleriOSV1)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_UShort)}", 0);
                    TextureCoder.Decode<R5_G6_B5_UShort>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
            }
        }

        public void EncodePtx<TLogger>(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, TLogger logger)
            where TLogger : ILogger
        {
            switch (format)
            {
                case 0:
                    logger.Log($"{nameof(PtxHandleriOSV1)}.{nameof(EncodePtx)} use format {nameof(B8_G8_R8_A8_UByte)}", 0);
                    TextureCoder.Encode<B8_G8_R8_A8_UByte>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 1:
                default:
                    logger.Log($"{nameof(PtxHandleriOSV1)}.{nameof(EncodePtx)} use format {nameof(R4_G4_B4_A4_UShort)}", 0);
                    TextureCoder.Encode<R4_G4_B4_A4_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 2:
                    logger.Log($"{nameof(PtxHandleriOSV1)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_UShort)}", 0);
                    TextureCoder.Encode<R5_G6_B5_UShort>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
            }
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            width = (uint)srcBitmap.Width;
            height = (uint)srcBitmap.Height;
            pitch = width << (format == 0u ? 2 : 1);
            alphaSize = 0u;
            return true;
        }
    }
}
