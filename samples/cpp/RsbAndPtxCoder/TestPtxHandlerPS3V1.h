#pragma once
#include "LibWindPop.h"

int WIND_API TestPtxHandlerPS3V1_GetPtxSize(int width, int height, int pitch, int format, int alphaSize);
int WIND_API TestPtxHandlerPS3V1_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format);
void WIND_API TestPtxHandlerPS3V1_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize);
void WIND_API TestPtxHandlerPS3V1_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize);
EResult WIND_API TestPtxHandlerPS3V1_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize);
