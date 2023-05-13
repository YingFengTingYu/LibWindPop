#include "TestPtxHandlerPS3V1.h"

int WIND_API TestPtxHandlerPS3V1_GetPtxSize(int width, int height, int pitch, int format, int alphaSize) {
	return height * pitch;
}

int WIND_API TestPtxHandlerPS3V1_GetPtxSizeWithoutAlpha(int width, int height, int pitch, int format) {
	return height * pitch;
}

void WIND_API TestPtxHandlerPS3V1_DecodePtx(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
	unsigned char* ptxDataPtrC = ptxDataPtr;
	unsigned char* bitmapDataPtrC = bitmapDataPtr;
	unsigned char* ptxDataPtrT;
	unsigned char* bitmapDataPtrT;
	int minWidth = ptxWidth < bitmapWidth ? ptxWidth : bitmapWidth;
	int minHeight = ptxHeight < bitmapHeight ? ptxHeight : bitmapHeight;
	int cpySize = minWidth * 4;
	int bitmapPitch = bitmapWidth * 4;
	for (int y = 0; y < minHeight; y++) {
		ptxDataPtrT = ptxDataPtrC;
		bitmapDataPtrT = bitmapDataPtrC;
		ptxDataPtrC += ptxPitch;
		bitmapDataPtrC += bitmapPitch;
		for (int x = 0; x < minWidth; x++) {
			bitmapDataPtrT[0] = ptxDataPtrT[3];
			bitmapDataPtrT[1] = ptxDataPtrT[2];
			bitmapDataPtrT[2] = ptxDataPtrT[1];
			bitmapDataPtrT[3] = ptxDataPtrT[0];
			bitmapDataPtrT += 4;
			ptxDataPtrT += 4;
		}
	}
}

void WIND_API TestPtxHandlerPS3V1_EncodePtx(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int ptxWidth, int ptxHeight, int ptxPitch, int ptxFormat, int ptxAlphaSize) {
	unsigned char* ptxDataPtrC = ptxDataPtr;
	unsigned char* bitmapDataPtrC = bitmapDataPtr;
	unsigned char* ptxDataPtrT;
	unsigned char* bitmapDataPtrT;
	int minWidth = ptxWidth < bitmapWidth ? ptxWidth : bitmapWidth;
	int minHeight = ptxHeight < bitmapHeight ? ptxHeight : bitmapHeight;
	int cpySize = minWidth * 4;
	int bitmapPitch = bitmapWidth * 4;
	for (int y = 0; y < minHeight; y++) {
		ptxDataPtrT = ptxDataPtrC;
		bitmapDataPtrT = bitmapDataPtrC;
		ptxDataPtrC += ptxPitch;
		bitmapDataPtrC += bitmapPitch;
		for (int x = 0; x < minWidth; x++) {
			ptxDataPtrT[3] = bitmapDataPtrT[0];
			ptxDataPtrT[2] = bitmapDataPtrT[1];
			ptxDataPtrT[1] = bitmapDataPtrT[2];
			ptxDataPtrT[0] = bitmapDataPtrT[3];
			bitmapDataPtrT += 4;
			ptxDataPtrT += 4;
		}
	}
}

EResult WIND_API TestPtxHandlerPS3V1_PeekEncodedPtxInfo(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int ptxFormat, int* ptxWidth, int* ptxHeight, int* ptxPitch, int* ptxAlphaSize) {
	*ptxWidth = bitmapWidth;
	*ptxHeight = bitmapHeight;
	*ptxPitch = bitmapWidth * 4;
	*ptxAlphaSize = 0;
	return Result_OK;
}
