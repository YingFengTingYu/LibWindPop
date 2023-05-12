using LibWindPop.Utils.Graphics;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Logger;
using System;

namespace LibWindPop.Images.PtxRsb.Handler
{
    public sealed unsafe class NativePtxHandler : IPtxRsbHandler
    {
        private readonly bool m_useExtend1AsAlphaSize;
        private readonly delegate* unmanaged[Stdcall]<int, int, int, int, int, int> m_getPtxSize;
        private readonly delegate* unmanaged[Stdcall]<int, int, int, int, int> m_getPtxSizeWithoutAlpha;
        private readonly delegate* unmanaged[Stdcall]<void*, int, void*, int, int, int, int, int, int, int, void> m_decodePtx;
        private readonly delegate* unmanaged[Stdcall]<void*, int, int, void*, int, int, int, int, int, int, void> m_encodePtx;
        private readonly delegate* unmanaged[Stdcall]<void*, int, int, int, int*, int*, int*, int*, int> m_peekEncodedPtxInfo;

        public NativePtxHandler(bool useExtend1AsAlphaSize, delegate* unmanaged[Stdcall]<int, int, int, int, int, int> getPtxSize, delegate* unmanaged[Stdcall]<int, int, int, int, int> getPtxSizeWithoutAlpha, delegate* unmanaged[Stdcall]<void*, int, void*, int, int, int, int, int, int, int, void> decodePtx, delegate* unmanaged[Stdcall]<void*, int, int, void*, int, int, int, int, int, int, void> encodePtx, delegate* unmanaged[Stdcall]<void*, int, int, int, int*, int*, int*, int*, int> peekEncodedPtxInfo)
        {
            m_useExtend1AsAlphaSize = useExtend1AsAlphaSize;
            m_getPtxSize = getPtxSize;
            m_getPtxSizeWithoutAlpha = getPtxSizeWithoutAlpha;
            m_decodePtx = decodePtx;
            m_encodePtx = encodePtx;
            m_peekEncodedPtxInfo = peekEncodedPtxInfo;
        }

        public bool UseExtend1AsAlphaSize => m_useExtend1AsAlphaSize;

        public uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize)
        {
            if (m_getPtxSize != null)
            {
                return (uint)m_getPtxSize((int)width, (int)height, (int)pitch, (int)format, (int)alphaSize);
            }
            return 0u;
        }

        public uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format)
        {
            if (m_getPtxSizeWithoutAlpha != null)
            {
                return (uint)m_getPtxSizeWithoutAlpha((int)width, (int)height, (int)pitch, (int)format);
            }
            return 0u;
        }

        public void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            if (m_decodePtx != null)
            {
                Span<YFColor> bitmapSpan = dstBitmap.AsSpan();
                fixed (byte* ptxDataPtr = ptxData)
                {
                    fixed (YFColor* bitmapDataPtr = bitmapSpan)
                    {
                        m_decodePtx(ptxDataPtr, ptxData.Length, bitmapDataPtr, dstBitmap.Width, dstBitmap.Height, (int)width, (int)height, (int)pitch, (int)format, (int)alphaSize);
                    }
                }
            }
        }

        public void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger)
        {
            if (m_encodePtx != null)
            {
                Span<YFColor> bitmapSpan = srcBitmap.AsSpan();
                fixed (byte* ptxDataPtr = ptxData)
                {
                    fixed (YFColor* bitmapDataPtr = bitmapSpan)
                    {
                        m_encodePtx(bitmapDataPtr, srcBitmap.Width, srcBitmap.Height, ptxDataPtr, ptxData.Length, (int)width, (int)height, (int)pitch, (int)format, (int)alphaSize);
                    }
                }
            }
        }

        public bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize)
        {
            if (m_peekEncodedPtxInfo != null)
            {
                Span<YFColor> bitmapSpan = srcBitmap.AsSpan();
                fixed (YFColor* bitmapDataPtr = bitmapSpan)
                {
                    int tWidth = 0, tHeight = 0, tPitch = 0, tAlphaSize = 0;
                    int ans = m_peekEncodedPtxInfo(bitmapDataPtr, srcBitmap.Width, srcBitmap.Height, (int)format, &tWidth, &tHeight, &tPitch, &tAlphaSize);
                    width = (uint)tWidth;
                    height = (uint)tHeight;
                    pitch = (uint)tPitch;
                    alphaSize = (uint)tAlphaSize;
                    return ans == 0;
                }
            }
            width = 0u;
            height = 0u;
            pitch = 0u;
            alphaSize = 0u;
            return false;
        }
    }
}
