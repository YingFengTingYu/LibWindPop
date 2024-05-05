using LibWindPop.Images.PtxRsb.Handler;
using LibWindPop.Utils.Graphics.Bitmap;
using LibWindPop.Utils.Logger;
using System;
using System.Collections.Generic;

namespace LibWindPop.Images.PtxRsb
{
    public static class PtxRsbHandlerManager
    {
        public static bool FIX_COMPRESSED_TEX_SIZE = true;

        private static readonly Dictionary<string, IPtxRsbHandler> m_ptxHandlerMap = new Dictionary<string, IPtxRsbHandler>
        {
            { nameof(PtxHandlerAndroidV1), new PtxHandlerAndroidV1() },
            { nameof(PtxHandlerAndroidV2), new PtxHandlerAndroidV2() },
            { nameof(PtxHandlerAndroidV3), new PtxHandlerAndroidV3() },
            { nameof(PtxHandleriOSV1), new PtxHandleriOSV1() },
            { nameof(PtxHandleriOSV2), new PtxHandleriOSV2() },
            { nameof(PtxHandleriOSV3), new PtxHandleriOSV3() },
            { nameof(PtxHandleriOSV4), new PtxHandleriOSV4() },
            { nameof(PtxHandleriOSV5), new PtxHandleriOSV5() },
            { nameof(PtxHandlerPS3V1), new PtxHandlerPS3V1() },
            { nameof(PtxHandlerPS4V1), new PtxHandlerPS4V1() },
            { nameof(PtxHandlerXbox360V1), new PtxHandlerXbox360V1() },
            { nameof(PtxHandlerXbox360V2), new PtxHandlerXbox360V2() },
            { nameof(PtxHandlerPVZ2CNAndroidV1), new PtxHandlerPVZ2CNAndroidV1() },
            { nameof(PtxHandlerPVZ2CNAndroidV2), new PtxHandlerPVZ2CNAndroidV2() },
            { nameof(PtxHandlerPVZ2CNAndroidV3), new PtxHandlerPVZ2CNAndroidV3() },
            { nameof(PtxHandlerPVZ2CNAndroidV4), new PtxHandlerPVZ2CNAndroidV4() },
            { nameof(PtxHandlerPVZ2CNAndroidV5), new PtxHandlerPVZ2CNAndroidV5() },
            { nameof(PtxHandlerPVZ2CNiOSV1), new PtxHandlerPVZ2CNiOSV1() },
            { nameof(PtxHandlerPVZ2CNiOSV2), new PtxHandlerPVZ2CNiOSV2() },
        };

        public static bool RegistHandler(string handlerName, IPtxRsbHandler handler)
        {
            return m_ptxHandlerMap.TryAdd(handlerName, handler);
        }

        public static IPtxRsbHandler GetHandlerFromId<TLogger>(string? handlerId, TLogger logger)
            where TLogger : ILogger
        {
            if (handlerId != null && m_ptxHandlerMap.TryGetValue(handlerId, out IPtxRsbHandler? handler) && handler != null)
            {
                return handler;
            }
            logger.LogError($"Cannot find ptx handler {handlerId}");
            return m_ptxHandlerMap[nameof(PtxHandlerAndroidV3)];
        }
    }

    public interface IPtxRsbHandler
    {
        bool UseExtend1AsAlphaSize { get; }

        uint GetPtxSize(uint width, uint height, uint pitch, uint format, uint alphaSize);

        uint GetPtxSizeWithoutAlpha(uint width, uint height, uint pitch, uint format);

        void DecodePtx(ReadOnlySpan<byte> ptxData, RefBitmap dstBitmap, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger);

        void EncodePtx(RefBitmap srcBitmap, Span<byte> ptxData, uint width, uint height, uint pitch, uint format, uint alphaSize, ILogger logger);

        bool PeekEncodedPtxInfo(RefBitmap srcBitmap, uint format, out uint width, out uint height, out uint pitch, out uint alphaSize);
    }
}
