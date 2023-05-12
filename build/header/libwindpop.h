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

WIND_IMPORT EResult WIND_API RsbUnpack(const char* rsbPath, const char* unpackPath, const char* ptxHandlerType, EBool useGroupFolder, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbUnpackU8(const char* rsbPath, const char* unpackPath, const char* ptxHandlerType, EBool useGroupFolder, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbAddContentPipeline(const char* unpackPath, const char* pipelineName, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbAddContentPipelineU8(const char* unpackPath, const char* pipelineName, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbPack(const char* unpackPath, const char* rsbPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API RsbPackU8(const char* unpackPath, const char* rsbPath, int logLevel, EBool throwException);

WIND_IMPORT EResult WIND_API PtxRsbDecode(const char* ptxPath, const char* pngPath, const char* ptxHandlerType, int width, int height, int pitch, int format, int alphaSize, int logLevel);

WIND_IMPORT EResult WIND_API PtxRsbDecodeU8(const char* ptxPath, const char* pngPath, const char* ptxHandlerType, int width, int height, int pitch, int format, int alphaSize, int logLevel);

WIND_IMPORT EResult WIND_API PtxRsbEncode(const char* pngPath, const char* ptxPath, const char* ptxHandlerType, int format, int logLevel, int* width, int* height, int* pitch, int* alphaSize);

WIND_IMPORT EResult WIND_API PtxRsbEncodeU8(const char* pngPath, const char* ptxPath, const char* ptxHandlerType, int format, int logLevel, int* width, int* height, int* pitch, int* alphaSize);

WIND_IMPORT EResult WIND_API PtxRsbRegistHandler(const char* ptxHandlerType, EBool useExtend1AsAlphaSize, int(WIND_API* getPtxSize)(int width, int height, int pitch, int format, int alphaSize), int(WIND_API* getPtxSizeWithoutAlpha)(int width, int height, int pitch, int format), void(WIND_API* decodePtx)(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int width, int height, int pitch, int format, int alphaSize), void(WIND_API* encodePtx)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int width, int height, int pitch, int format, int alphaSize), EResult(WIND_API* peekEncodedPtxInfo)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int format, int* widthPtr, int* heightPtr, int* pitchPtr, int* alphaSizePtr));

WIND_IMPORT EResult WIND_API PtxRsbRegistHandlerU8(const char* ptxHandlerType, EBool useExtend1AsAlphaSize, int(WIND_API* getPtxSize)(int width, int height, int pitch, int format, int alphaSize), int(WIND_API* getPtxSizeWithoutAlpha)(int width, int height, int pitch, int format), void(WIND_API* decodePtx)(void* ptxDataPtr, int ptxDataSize, void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int width, int height, int pitch, int format, int alphaSize), void(WIND_API* encodePtx)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, void* ptxDataPtr, int ptxDataSize, int width, int height, int pitch, int format, int alphaSize), EResult(WIND_API* peekEncodedPtxInfo)(void* bitmapDataPtr, int bitmapWidth, int bitmapHeight, int format, int* widthPtr, int* heightPtr, int* pitchPtr, int* alphaSizePtr));

WIND_IMPORT size_t WIND_API GetErrorSize();

WIND_IMPORT size_t WIND_API GetErrorSizeU8();

WIND_IMPORT EResult WIND_API GetError(char* buffer);

WIND_IMPORT EResult WIND_API GetErrorU8(char* buffer);
