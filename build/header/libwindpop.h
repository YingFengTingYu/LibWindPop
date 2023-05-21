#pragma once
#ifndef LIBWINDPOP_H
#define LIBWINDPOP_H
#define WIND_IMPORT
#define WIND_API __stdcall

typedef enum ENUMBool
{
    Bool_False = 0,
    Bool_True = 1,
} EBool;

typedef enum ENUMResult
{
    Result_OK = 0,
    Result_Error = 1,
} EResult;

static char PtxHandlerAndroidV1[] = "PtxHandlerAndroidV1";
static char PtxHandlerAndroidV2[] = "PtxHandlerAndroidV2";
static char PtxHandlerAndroidV3[] = "PtxHandlerAndroidV3";
static char PtxHandleriOSV1[] = "PtxHandleriOSV1";
static char PtxHandleriOSV2[] = "PtxHandleriOSV2";
static char PtxHandleriOSV3[] = "PtxHandleriOSV3";
static char PtxHandleriOSV4[] = "PtxHandleriOSV4";
static char PtxHandleriOSV5[] = "PtxHandleriOSV5";
static char PtxHandlerPS3V1[] = "PtxHandlerPS3V1";
static char PtxHandlerPS4V1[] = "PtxHandlerPS4V1";
static char PtxHandlerXbox360V1[] = "PtxHandlerXbox360V1";
static char PtxHandlerXbox360V2[] = "PtxHandlerXbox360V2";
static char PtxHandlerPVZ2CNAndroidV1[] = "PtxHandlerPVZ2CNAndroidV1";
static char PtxHandlerPVZ2CNAndroidV2[] = "PtxHandlerPVZ2CNAndroidV2";
static char PtxHandlerPVZ2CNAndroidV3[] = "PtxHandlerPVZ2CNAndroidV3";
static char PtxHandlerPVZ2CNAndroidV4[] = "PtxHandlerPVZ2CNAndroidV4";
static char PtxHandlerPVZ2CNAndroidV5[] = "PtxHandlerPVZ2CNAndroidV5";
static char PtxHandlerPVZ2CNiOSV1[] = "PtxHandlerPVZ2CNiOSV1";
static char PtxHandlerPVZ2CNiOSV2[] = "PtxHandlerPVZ2CNiOSV2";
static char RsbPipelineUpdateRsgCache[] = "UpdateRsgCache";
static char RsbPipelineEncodePtxFromPng[] = "EncodePtxFromPng";
static char PakPipelinePakRebuildFile[] = "PakRebuildFile";

WIND_IMPORT EResult WIND_API RsbUnpack(const char* rsbPath, const char* unpackPath, const char* ptxHandlerType, EBool useGroupFolder, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbUnpackU8(const char* rsbPath, const char* unpackPath, const char* ptxHandlerType, EBool useGroupFolder, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbAddContentPipeline(const char* unpackPath, const char* pipelineName, EBool atFirst, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbAddContentPipelineU8(const char* unpackPath, const char* pipelineName, EBool atFirst, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbPack(const char* unpackPath, const char* rsbPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbPackU8(const char* unpackPath, const char* rsbPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbRegistContentPipeline(const char* pipelineName, void(WIND_API* onStartBuild)(const char* unpackPath), void(WIND_API* onEndBuild)(const char* rsbPath), void(WIND_API* onAdd)(const char* unpackPath));

WIND_IMPORT EResult WIND_API RsbRegistContentPipelineU8(const char* pipelineName, void(WIND_API* onStartBuild)(const char* unpackPath), void(WIND_API* onEndBuild)(const char* rsbPath), void(WIND_API* onAdd)(const char* unpackPath));

WIND_IMPORT EResult WIND_API PtxRsbDecode(const char* ptxPath, const char* pngPath, const char* ptxHandlerType, int width, int height, int pitch, int format, int alphaSize, int logLevel);

WIND_IMPORT EResult WIND_API PtxRsbDecodeU8(const char* ptxPath, const char* pngPath, const char* ptxHandlerType, int width, int height, int pitch, int format, int alphaSize, int logLevel);

WIND_IMPORT EResult WIND_API PtxRsbEncode(const char* pngPath, const char* ptxPath, const char* ptxHandlerType, int format, int logLevel, int* width, int* height, int* pitch, int* alphaSize);

WIND_IMPORT EResult WIND_API PtxRsbEncodeU8(const char* pngPath, const char* ptxPath, const char* ptxHandlerType, int format, int logLevel, int* width, int* height, int* pitch, int* alphaSize);

WIND_IMPORT EResult WIND_API PtxRsbRegistHandler(const char* ptxHandlerType, EBool useExtend1AsAlphaSize, int(WIND_API* getPtxSize)(int width, int height, int pitch, int format, int alphaSize), int(WIND_API* getPtxSizeWithoutAlpha)(int width, int height, int pitch, int format), void(WIND_API* decodePtx)(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int width, int height, int pitch, int format, int alphaSize), void(WIND_API* encodePtx)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int width, int height, int pitch, int format, int alphaSize), EResult(WIND_API* peekEncodedPtxInfo)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int format, int* widthPtr, int* heightPtr, int* pitchPtr, int* alphaSizePtr));

WIND_IMPORT EResult WIND_API PtxRsbRegistHandlerU8(const char* ptxHandlerType, EBool useExtend1AsAlphaSize, int(WIND_API* getPtxSize)(int width, int height, int pitch, int format, int alphaSize), int(WIND_API* getPtxSizeWithoutAlpha)(int width, int height, int pitch, int format), void(WIND_API* decodePtx)(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int width, int height, int pitch, int format, int alphaSize), void(WIND_API* encodePtx)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int width, int height, int pitch, int format, int alphaSize), EResult(WIND_API* peekEncodedPtxInfo)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int format, int* widthPtr, int* heightPtr, int* pitchPtr, int* alphaSizePtr));

WIND_IMPORT EResult WIND_API PakUnpack(const char* pakPath, const char* unpackPath, EBool useZlib, EBool useAlign, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PakUnpackU8(const char* pakPath, const char* unpackPath, EBool useZlib, EBool useAlign, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PakAddContentPipeline(const char* unpackPath, const char* pipelineName, EBool atFirst, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PakAddContentPipelineU8(const char* unpackPath, const char* pipelineName, EBool atFirst, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PakPack(const char* unpackPath, const char* pakPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PakPackU8(const char* unpackPath, const char* pakPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PakRegistContentPipeline(const char* pipelineName, void(WIND_API* onStartBuild)(const char* unpackPath), void(WIND_API* onEndBuild)(const char* pakPath), void(WIND_API* onAdd)(const char* unpackPath));

WIND_IMPORT EResult WIND_API PakRegistContentPipelineU8(const char* pipelineName, void(WIND_API* onStartBuild)(const char* unpackPath), void(WIND_API* onEndBuild)(const char* pakPath), void(WIND_API* onAdd)(const char* unpackPath));

WIND_IMPORT int WIND_API GetErrorSize();

WIND_IMPORT int WIND_API GetErrorSizeU8();

WIND_IMPORT EResult WIND_API GetError(char* buffer);

WIND_IMPORT EResult WIND_API GetErrorU8(char* buffer);

WIND_IMPORT int WIND_API GetLibVersion();
#endif
