using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Graphics.Texture;
using LibWindPop.Utils.Graphics.Texture.Coder;
using LibWindPop.Utils.Logger;
using System;

namespace LibWindPop.Images.PtxRsb.Handler
{
    /// <summary>
    /// Bejeweled 3
    /// PopCap use GCM instead of PSGL on PS3. Only support A8_R8_G8_B8_UByte.
    /// </summary>
    public sealed class PtxHandlerPS3V1 : IPtxRsbHandler
    {
        public bool UseExtend1AsAlphaSize => false;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            return GetPtxSizeWithoutAlpha(width, height, pitch, format);
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            // from Bejeweled 3 PS3 0x217DA4(Sexy::PS3ResStreamsDriver::GetGPUDataSizeForTexture) (Need to create function by yourself)
            return pitch * height;
        }

        public void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            logger.Log($"{nameof(PtxHandlerPS3V1)}.{nameof(DecodePtx)} use format {nameof(A8_R8_G8_B8_UIntBE)}", 0);
            TextureCoder.Decode<A8_R8_G8_B8_UIntBE>(ptxData, (int)width, (int)height, (int)pitch, dstBitmap);
        }

        public void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            logger.Log($"{nameof(PtxHandlerPS3V1)}.{nameof(EncodePtx)} use format {nameof(A8_R8_G8_B8_UIntBE)}", 0);
            TextureCoder.Encode<A8_R8_G8_B8_UIntBE>(srcBitmap, ptxData, (int)width, (int)height, (int)pitch);
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            width = (uint)srcBitmap.Width;
            height = (uint)srcBitmap.Height;
            pitch = width << 2;
            alphaSize = 0u;
            return true;
        }
    }
}
