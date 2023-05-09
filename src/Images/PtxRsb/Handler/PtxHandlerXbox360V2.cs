using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Logger;
using System;

namespace LibWindPop.Images.PtxRsb.Handler
{
    /// <summary>
    /// Peggle 2
    /// </summary>
    public sealed class PtxHandlerXbox360V2 : IPtxRsbHandler
    {
        public bool UseExtend1AsAlphaSize => false;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            return GetPtxSizeWithoutAlpha(width, height, pitch, format);
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            // from Peggle 2 Xbox360 0x8245E558(Sexy::Xbox360ResStreamsDriver::GetGPUDataSizeForTexture) (Need to create function by yourself)
            if (format == 5u)
            {
                return ((height + 3u) & 0xFFFFFFFCu) * pitch;
            }
            return pitch * height;
        }

        public void DecodePtx<TLogger>(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, TLogger logger)
            where TLogger : ILogger
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(DecodePtx)} use format {nameof(A8_R8_G8_B8_UIntBE)}", 0);
                    TextureCoder.Decode<A8_R8_G8_B8_UIntBE>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 1u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(DecodePtx)} use format {nameof(A4_R4_G4_B4_UShortBE)}", 0);
                    TextureCoder.Decode<A4_R4_G4_B4_UShortBE>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(DecodePtx)} use format {nameof(R5_G6_B5_UShortBE)}", 0);
                    TextureCoder.Decode<R5_G6_B5_UShortBE>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
                    break;
                case 5u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(DecodePtx)} use format {nameof(R8_G8_B8_A8_DXT5_UByte)}", 0);
                    TextureCoder.Decode<R8_G8_B8_A8_DXT5_UByte>(ptxData, (int)width, (int)height, (int)(pitch << 2), dstBitmap);
                    break;
                default:
                    // Unknow gray texture?
                    logger.LogError($"{nameof(PtxHandlerXbox360V2)}.{nameof(DecodePtx)} does not support format {format}", 0, true);
                    break;
            }
        }

        public void EncodePtx<TLogger>(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, TLogger logger)
            where TLogger : ILogger
        {
            switch (format)
            {
                case 0u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(EncodePtx)} use format {nameof(A8_R8_G8_B8_UIntBE)}", 0);
                    TextureCoder.Encode<A8_R8_G8_B8_UIntBE>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 1u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(EncodePtx)} use format {nameof(A4_R4_G4_B4_UShortBE)}", 0);
                    TextureCoder.Encode<A4_R4_G4_B4_UShortBE>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 2u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(EncodePtx)} use format {nameof(R5_G6_B5_UShortBE)}", 0);
                    TextureCoder.Encode<R5_G6_B5_UShortBE>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
                    break;
                case 5u:
                    logger.Log($"{nameof(PtxHandlerXbox360V2)}.{nameof(EncodePtx)} use format {nameof(R8_G8_B8_A8_DXT5_UByte)}", 0);
                    TextureCoder.Encode<R8_G8_B8_A8_DXT5_UByte>(srcBitmap, ptxData, (int)width, (int)height, (int)(pitch << 2));
                    break;
                default:
                    // Unknow gray texture?
                    logger.LogError($"{nameof(PtxHandlerXbox360V2)}.{nameof(EncodePtx)} does not support format {format}", 0, true);
                    break;
            }
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            if (format == 5u)
            {
                width = (uint)srcBitmap.Width;
                height = (uint)srcBitmap.Height;
                uint alignWidth = width;
                if ((alignWidth & 3u) != 0u)
                {
                    alignWidth |= 3u;
                    alignWidth++;
                }
                pitch = alignWidth;
                alphaSize = 0u;
                return true;
            }
            // Unknow gray texture?
            if (format > 2u || format < 0u) // unsigned value will never smaller than 0u!
            {
                width = 0u;
                height = 0u;
                pitch = 0u;
                alphaSize = 0u;
                return false;
            }
            width = (uint)srcBitmap.Width;
            height = (uint)srcBitmap.Height;
            pitch = width << (format == 0u ? 2 : 1);
            if ((pitch & 0xFFu) != 0u)
            {
                pitch |= 0xFFu;
                pitch++;
            }
            alphaSize = 0u;
            return true;
        }
    }
}
