using LibWindPop.Utils.Graphics.Bitmap;
using PVRTexLib;
using System;
using System.Runtime.InteropServices;

namespace LibWindPop.Utils.Graphics.Texture.Shared
{
    internal class PVRTexLibHelper
    {
        public static bool Encode(RefBitmap srcBitmap, PVRTexLibPixelFormat inFormat, ReadOnlySpan<byte> dstData)
        {
            if (!TextureSettings.UsePVRTexLib)
            {
                return false;
            }
            using (PVRTextureHeader header = new PVRTextureHeader(PVRDefine.PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), (uint)srcBitmap.Width, (uint)srcBitmap.Height, 1, 1, 1, 1, PVRTexLibColourSpace.sRGB, PVRTexLibVariableType.UnsignedByteNorm, false))
            {
                unsafe
                {
                    fixed (byte* dstTexDataPtr = &dstData[0])
                    {
                        fixed (YFColor* bitmapDataPtr = &srcBitmap.AsSpan()[0])
                        {
                            using (PVRTexture tex = new PVRTexture(header, bitmapDataPtr))
                            {
                                if (tex.GetTextureDataSize() != 0)
                                {
                                    if (tex.Transcode((ulong)inFormat, PVRTexLibVariableType.UnsignedByteNorm, PVRTexLibColourSpace.sRGB, 0, GetDither()))
                                    {
                                        NativeMemory.Copy(tex.GetTextureDataPointer(0), dstTexDataPtr, (nuint)tex.GetTextureDataSize(0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static bool Decode(ReadOnlySpan<byte> srcData, PVRTexLibPixelFormat inFormat, RefBitmap dstBitmap)
        {
            if (!TextureSettings.UsePVRTexLib)
            {
                return false;
            }
            using (PVRTextureHeader header = new PVRTextureHeader((ulong)inFormat, (uint)dstBitmap.Width, (uint)dstBitmap.Height, 1, 1, 1, 1, PVRTexLibColourSpace.sRGB, PVRTexLibVariableType.UnsignedByteNorm, false))
            {
                unsafe
                {
                    fixed (byte* rawTexDataPtr = &srcData[0])
                    {
                        fixed (YFColor* bitmapDataPtr = &dstBitmap.AsSpan()[0])
                        {
                            using (PVRTexture tex = new PVRTexture(header, rawTexDataPtr))
                            {
                                if (tex.GetTextureDataSize() != 0)
                                {
                                    if (tex.Transcode(PVRDefine.PVRTGENPIXELID4('b', 'g', 'r', 'a', 8, 8, 8, 8), PVRTexLibVariableType.UnsignedByteNorm, PVRTexLibColourSpace.sRGB, GetQuality(inFormat), GetDither()))
                                    {
                                        NativeMemory.Copy(tex.GetTextureDataPointer(0), bitmapDataPtr, (nuint)tex.GetTextureDataSize(0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static bool GetDither()
        {
            return TextureSettings.PVRTexLibDoDither;
        }

        public static PVRTexLibCompressorQuality GetQuality(PVRTexLibPixelFormat inFormat)
        {
            PVRTexLibCompressorQuality quality = 0;
            if (inFormat == PVRTexLibPixelFormat.PVRTCI_2bpp_RGB
                || inFormat == PVRTexLibPixelFormat.PVRTCI_4bpp_RGB
                || inFormat == PVRTexLibPixelFormat.PVRTCI_2bpp_RGBA
                || inFormat == PVRTexLibPixelFormat.PVRTCI_4bpp_RGBA
                || inFormat == PVRTexLibPixelFormat.PVRTCII_2bpp
                || inFormat == PVRTexLibPixelFormat.PVRTCII_4bpp
                || inFormat == PVRTexLibPixelFormat.PVRTCI_HDR_6bpp
                || inFormat == PVRTexLibPixelFormat.PVRTCI_HDR_8bpp
                || inFormat == PVRTexLibPixelFormat.PVRTCII_HDR_6bpp
                || inFormat == PVRTexLibPixelFormat.PVRTCII_HDR_8bpp)
            {
                if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.Fast)
                {
                    quality = PVRTexLibCompressorQuality.PVRTCFastest;
                }
                else if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.Normal)
                {
                    quality = PVRTexLibCompressorQuality.PVRTCNormal;
                }
                else if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.High)
                {
                    quality = PVRTexLibCompressorQuality.PVRTCVeryHigh;
                }
                else if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.Best)
                {
                    quality = PVRTexLibCompressorQuality.PVRTCBest;
                }
            }
            else if (inFormat == PVRTexLibPixelFormat.ETC1
                || inFormat == PVRTexLibPixelFormat.ETC2_RGB
                || inFormat == PVRTexLibPixelFormat.ETC2_RGBA
                || inFormat == PVRTexLibPixelFormat.ETC2_RGB_A1
                || inFormat == PVRTexLibPixelFormat.EAC_R11
                || inFormat == PVRTexLibPixelFormat.EAC_RG11)
            {
                if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.Fast)
                {
                    quality = PVRTexLibCompressorQuality.ETCFast;
                }
                else if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.Normal)
                {
                    quality = PVRTexLibCompressorQuality.ETCNormal;
                }
                else if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.High)
                {
                    quality = PVRTexLibCompressorQuality.ETCSlow;
                }
                else if (TextureSettings.PVRTexLibQuality == TextureSettings.PVRTexLibTextureQuality.Best)
                {
                    quality = PVRTexLibCompressorQuality.ETCSlow;
                }
            }
            return quality;
        }
    }
}
