#pragma once
#include "LibWindPop.h"

#ifdef __cplusplus
#include "PVRTexLib.hpp"
using namespace pvrtexlib;

inline int Align(int value, int ali) {
    int ove = value % ali;
    if (ove) {
        value += ali - ove;
    }
    return value;
}

inline int IsPowerOfTwo(int value) {
    return (value & (value - 1)) == 0;
}

extern "C" {
#endif
    int WIND_API PtxHandlerAndroidV3H_GetPtxSize(int width, int height, int pitch, int format, int alphaSize);
    int WIND_API PtxHandlerAndroidV3H_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format);
    void WIND_API PtxHandlerAndroidV3H_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize);
    void WIND_API PtxHandlerAndroidV3H_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize);
    EResult WIND_API PtxHandlerAndroidV3H_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize);
    int WIND_API PtxHandleriOSV5H_GetPtxSize(int width, int height, int pitch, int format, int alphaSize);
    int WIND_API PtxHandleriOSV5H_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format);
    void WIND_API PtxHandleriOSV5H_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize);
    void WIND_API PtxHandleriOSV5H_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize);
    EResult WIND_API PtxHandleriOSV5H_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize);
#ifdef __cplusplus          
}
#endif
